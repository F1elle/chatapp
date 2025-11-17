namespace ChatApp.Chat.Features.Chat.Abstractions;

public interface IChatAccessService
{
    Task<bool> CanAccessChatAsync(Guid userId, Guid chatId, CancellationToken ct);
}