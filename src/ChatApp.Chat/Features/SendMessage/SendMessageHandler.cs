using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Features.Common.Contracts;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.SendMessage;

public class SendMessageHandler : IHandler<SendMessageCommand, Result<MessageDto, ChatError>>
{
    private readonly ChatDbContext _dbContext;
    private readonly IChatAccessService _accessService;
    private readonly ILogger<SendMessageHandler> _logger;

    public SendMessageHandler(
        ChatDbContext dbContext,
        IChatAccessService chatAccessService,
        ILogger<SendMessageHandler> logger
    )
    {
        _dbContext = dbContext;
        _accessService = chatAccessService;
        _logger = logger;
    }

    public async Task<Result<MessageDto, ChatError>> Handle(
        SendMessageCommand command,
        CancellationToken ct
    )
    {
        var validationResult = SendMessageValidator.Validate(command);

        if (!validationResult)
        {
            return ChatError.InvalidMessage;
        }

        var participantId = await _accessService.GetParticipantIdAsync(
            command.SenderId,
            command.ChatId,
            ct
        );

        if (participantId == null)
        {
            return ChatError.NotChatParticipant;
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var message = Message.CreateTextMessage(
                    command.SenderId,
                    command.ChatId,
                    command.Content
                );

                _logger.LogInformation(
                    "User with Id {Id} is trying to send a message {Message}",
                    command.SenderId,
                    command.Content
                );

                _dbContext.Add(message);

                await _dbContext.SaveChangesAsync(ct);

                var chat = await _dbContext
                    .Chats.Where(c => c.Id == command.ChatId)
                    .FirstOrDefaultAsync(ct);

                if (chat is { } c)
                {
                    c.UpdateLastMessage(message);
                }

                // await _dbContext
                //     .ChatParticipants.Where(cp =>
                //         cp.ChatId == request.ChatId && cp.UserId == request.SenderId
                //     )
                //     .ExecuteUpdateAsync(
                //         s => s.SetProperty(p => p.LastReadMessageId, message.Id),
                //         ct
                //     );

                await _dbContext.SaveChangesAsync(ct);

                await transaction.CommitAsync(CancellationToken.None);

                return new MessageDto(
                    message.Id,
                    message.SenderId,
                    message.Content,
                    message.Type,
                    message.ChatId,
                    message.SentAt,
                    message.AttachmentIds
                );
            }
            catch (OperationCanceledException)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message and update counters");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        });
    }
}
