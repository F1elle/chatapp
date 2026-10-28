using ChatApp.Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.Common;

public sealed class ChatAccessService : IChatAccessService
{
    private readonly ChatDbContext _dbContext;

    public ChatAccessService(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid?> GetParticipantIdAsync(Guid userId, Guid chatId, CancellationToken ct)
    {
        var chatParticipant = await _dbContext
            .ChatParticipants.Where(cp => cp.ChatId == chatId && cp.UserId == userId)
            .Select(cp => cp.Id)
            .FirstOrDefaultAsync(ct);

        return chatParticipant == Guid.Empty ? null : chatParticipant;
    }
}
