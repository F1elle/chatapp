using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.CreateChat;

public class CreateChatHandler : IHandler<CreateChatCommand, Result<CreateChatResult, ChatError>>
{
    private readonly ILogger<CreateChatHandler> _logger;
    private readonly ChatDbContext _dbContext;

    public CreateChatHandler(ILogger<CreateChatHandler> logger, ChatDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<CreateChatResult, ChatError>> Handle(
        CreateChatCommand command,
        CancellationToken ct
    )
    {
        if (command.ParticipantIds.Count == 0)
            return ChatError.EmptyParticipantList;

        var chat = Domain.Chat.CreateGroupChat(command.CreatedBy, command.Name);

        _logger.LogInformation(
            "Trying to create chat {ChatName} with ID {ChatId}",
            chat.Name,
            chat.Id
        );

        var participants = command
            .ParticipantIds.Select(userId => new Domain.ChatParticipant(userId, chat.Id))
            .ToList();

        _logger.LogInformation(
            "Adding {ParticipantCount} participants to chat {ChatId}",
            participants.Count,
            chat.Id
        );

        _dbContext.Chats.Add(chat);
        _dbContext.ChatParticipants.AddRange(participants);
        await _dbContext.SaveChangesAsync(ct);

        return new CreateChatResult(chat.Id);
    }
}
