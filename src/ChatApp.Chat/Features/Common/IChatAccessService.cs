namespace ChatApp.Chat.Features.Common;

public interface IChatAccessService
{
    Task<Guid?> GetParticipantIdAsync(Guid userId, Guid chatId, CancellationToken ct);
}
