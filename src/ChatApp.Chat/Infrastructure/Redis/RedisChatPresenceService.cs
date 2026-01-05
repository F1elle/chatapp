using System.Data.Common;
using ChatApp.Chat.Features.Abstractions;
using StackExchange.Redis;

namespace ChatApp.Chat.Infrastructure.Redis;

public class RedisChatPresenceService : IChatPresenceService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisChatPresenceService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    
    public async Task<HashSet<Guid>> GetActiveParticipantsAsync(Guid chatId, CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        var activeRaw = await db.SetMembersAsync(RedisKeys.ChatActive(chatId));

        var activeParticipants = new HashSet<Guid>();
        foreach (var raw in activeRaw)
        {
            if (Guid.TryParse(raw.ToString(), out var parsed))
                activeParticipants.Add(parsed);
        }

        return activeParticipants;
    }

    public Task MarkActiveAsync(Guid chatId, Guid userId, CancellationToken ct)
        => _redis.GetDatabase().SetAddAsync(RedisKeys.ChatActive(chatId), userId.ToString());

    public Task MarkInactiveAsync(Guid chatId, Guid userId, CancellationToken ct)
        => _redis.GetDatabase().SetRemoveAsync(RedisKeys.ChatActive(chatId), userId.ToString());
}