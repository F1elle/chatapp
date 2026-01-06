using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.CreateChat;

public class CreateChatHandler : IHandler<CreateChatRequest, Result<CreateChatResponse>>
{
    private readonly ILogger<CreateChatHandler> _logger;
    private readonly ChatDbContext _dbContext;

    public CreateChatHandler(
        ILogger<CreateChatHandler> logger,
        ChatDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext; 
    }

    public async Task<Result<CreateChatResponse>> Handle(CreateChatRequest request, CancellationToken ct)
    {
        var chat = Domain.Chat.CreateGroupChat(request.CreatedBy, request.Name);

        _logger.LogInformation("Creating chat {ChatName} with ID {ChatId}", chat.Name, chat.Id);

        var participants = request.ParticipantIds.Select(userId => 
            new Domain.ChatParticipant(userId, chat.Id)).ToList();

        _logger.LogInformation("Adding {ParticipantCount} participants to chat {ChatId}", participants.Count, chat.Id);

        
        chat.ChatParticipants.AddRange(participants);
        _dbContext.Chats.Add(chat);
        var changes = await _dbContext.SaveChangesAsync(ct);

        return changes > 0 
            ? new CreateChatResponse()
            : Result.Failure<CreateChatResponse>("Failed to create chat");
    }
}