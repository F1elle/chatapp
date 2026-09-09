using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.CreateChat;

public class CreateChatHandler : IHandler<CreateChatRequest, Result<CreateChatResponse, ChatError>>
{
    private readonly ILogger<CreateChatHandler> _logger;
    private readonly ChatDbContext _dbContext;

    public CreateChatHandler(ILogger<CreateChatHandler> logger, ChatDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<CreateChatResponse, ChatError>> Handle(
        CreateChatRequest request,
        CancellationToken ct
    )
    {
        if (request.ParticipantIds.Count == 0)
            return ChatError.EmptyParticipantList;

        var chat = Domain.Chat.CreateGroupChat(request.CreatedBy, request.Name);

        _logger.LogInformation(
            "Trying to create chat {ChatName} with ID {ChatId}",
            chat.Name,
            chat.Id
        );

        var participants = request
            .ParticipantIds.Select(userId => new Domain.ChatParticipant(userId, chat.Id))
            .ToList();

        _logger.LogInformation(
            "Adding {ParticipantCount} participants to chat {ChatId}",
            participants.Count,
            chat.Id
        );

        chat.ChatParticipants.AddRange(participants);
        _dbContext.Chats.Add(chat);
        await _dbContext.SaveChangesAsync(ct);

        return new CreateChatResponse(chat.Id);
    }
}
