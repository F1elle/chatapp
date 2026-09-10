namespace ChatApp.Chat.Features.Abstractions;

public interface IChatAccessService 
{
    Task<Guid?> GetParticipantIdAsync(Guid userId, Guid chatId, CancellationToken ct);
}