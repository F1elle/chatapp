namespace ChatApp.Chat.Features.Abstractions;

public interface IChatPresenceService
{
    Task<HashSet<Guid>> GetActiveParticipantsAsync(Guid chatId, CancellationToken ct);
    Task MarkActiveAsync(Guid chatId, Guid userId, CancellationToken ct); // TODO: chatId is unnecessary
    Task MarkInactiveAsync(Guid chatId, Guid userId, CancellationToken ct); // TODO: chatId is unnecessary
}