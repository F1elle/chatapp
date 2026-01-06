using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Domain;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.JoinChat;

public class JoinChatHandler : IHandler<JoinChatRequest, Result<JoinChatResponse>>
{
    private readonly ChatDbContext _dbContext;

    public JoinChatHandler(
        ChatDbContext dbContext)
    {   
        _dbContext = dbContext;
    }

    public async Task<Result<JoinChatResponse>> Handle(JoinChatRequest request, CancellationToken ct)
    {
        ChatParticipant chatParticipant = new(request.UserId, request.ChatId);

        _dbContext.Add(chatParticipant);
        await _dbContext.SaveChangesAsync(ct);

        return new JoinChatResponse();
    }
}