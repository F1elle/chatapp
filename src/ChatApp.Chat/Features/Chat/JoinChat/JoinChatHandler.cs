using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Chat.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.Chat.JoinChat;

public class JoinChatHandler
{
    private readonly ChatDbContext _dbContext;

    public JoinChatHandler(
        ChatDbContext dbContext)
    {   
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(JoinChatRequest request, CancellationToken ct)
    {
        ChatParticipant chatParticipant = new() 
        {
            UserId = request.UserId, 
            ChatId = request.ChatId
        };

        _dbContext.Add(chatParticipant);
        await _dbContext.AddAsync(ct);

        return Result.Success();
    }
}