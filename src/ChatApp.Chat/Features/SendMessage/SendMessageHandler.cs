using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.SendMessage;

// TODO: publish sent message to rabbitMQ and pick it up in hub

public class SendMessageHandler : IHandler<SendMessageCommand, Result<Guid, ChatError>>
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

    public async Task<Result<Guid, ChatError>> Handle(
        SendMessageCommand command,
        CancellationToken ct
    )
    {
        var validationResult = new SendMessageValidator().Validate(command); // TODO: inject

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

        throw new NotImplementedException();

        // var strategy = _dbContext.Database.CreateExecutionStrategy();
        //
        // try
        // {
        //     return await strategy.ExecuteAsync(async () =>
        //     {
        //         using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        //
        //         var message = Message.CreateTextMessage(
        //             (Guid)participantId,
        //             command.ChatId,
        //             command.Content
        //         );
        //
        //         _logger.LogInformation(
        //             "User with Id {Id} is trying to send a message {Message}",
        //             command.SenderId,
        //             command.Content
        //         );
        //
        //         _dbContext.Add(message);
        //
        //         await _dbContext.SaveChangesAsync(ct);
        //
        //         await _dbContext
        //             .Chats.Where(c => c.Id == request.ChatId)
        //             .ExecuteUpdateAsync(
        //                 s =>
        //                     s.SetProperty(c => c.LastMessageId, message.Id)
        //                         .SetProperty(c => c.LastMessageAt, message.SentAt),
        //                 ct
        //             );
        //
        //         await _dbContext
        //             .ChatParticipants.Where(cp =>
        //                 cp.ChatId == request.ChatId && cp.UserId == request.SenderId
        //             )
        //             .ExecuteUpdateAsync(
        //                 s => s.SetProperty(p => p.LastReadMessageId, message.Id),
        //                 ct
        //             );
        //
        //         await transaction.CommitAsync(ct);
        //
        //         // defining inactive users
        //         var participants = await _dbContext
        //             .ChatParticipants.Where(cp =>
        //                 cp.ChatId == request.ChatId && cp.UserId != request.SenderId
        //             )
        //             .Select(cp => cp.UserId) // TODO: maybe userID
        //             .ToListAsync(ct);
        //
        //         var activeParticipants = await _presenceService.GetActiveParticipantsAsync(
        //             request.ChatId,
        //             ct
        //         );
        //
        //         var inactiveParticipantIds = participants.Except(activeParticipants).ToList();
        //
        //         return new SendMessageResponse(
        //             new MessageDto(
        //                 message.Id,
        //                 message.ChatId,
        //                 new ChatParticipantDto((Guid)participantId, request.SenderId),
        //                 message.Type,
        //                 message.Content,
        //                 message.SentAt
        //             ),
        //             inactiveParticipantIds
        //         );
        //     });
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Failed to send message and update counters");
        //     return Result.Failure<SendMessageResponse>("Database error");
        // }
    }
}
