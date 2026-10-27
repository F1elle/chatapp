namespace ChatApp.Chat.Features.Common.Abstractions;

public interface IChatAccessService
{
    Task<Guid?> GetParticipantIdAsync(Guid userId, Guid chatId, CancellationToken ct);
}
