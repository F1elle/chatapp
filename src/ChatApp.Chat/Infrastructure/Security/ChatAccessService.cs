using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Infrastructure.Security;

public sealed class ChatAccessService : IChatAccessService
{
    private readonly ChatDbContext _dbContext;

    public ChatAccessService(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CanAccessChatAsync(Guid userId, Guid chatId, CancellationToken ct)
    {
        return await _dbContext.ChatParticipants.AnyAsync(
            cp => cp.ChatId == chatId && cp.UserId == userId, ct);
    }
}