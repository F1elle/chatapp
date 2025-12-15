using ChatApp.Chat.Features.Chat.JoinChat;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.Chat.CreateChat;

public class CreateChatHandler
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

    public async Task<Result> Handle(CreateChatRequest request, CancellationToken ct)
    {
        var chat = new Domain.Chat
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy,
            Type = request.Type
        };

        _logger.LogInformation("Creating chat {ChatName} with ID {ChatId}", chat.Name, chat.Id);

        var participants = request.ParticipantIds.Select(userId => new Domain.ChatParticipant
        {
            ChatId = chat.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        }).ToList();

        _logger.LogInformation("Adding {ParticipantCount} participants to chat {ChatId}", participants.Count, chat.Id);

        _dbContext.Chats.Add(chat);
        _dbContext.ChatParticipants.AddRange(participants);
        var changes = await _dbContext.SaveChangesAsync(ct);

        return changes > 0 
            ? Result.Success()
            : Result.Failure("Failed to create chat");
    }
}