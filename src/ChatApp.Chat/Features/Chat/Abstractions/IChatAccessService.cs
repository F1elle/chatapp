namespace ChatApp.Chat.Features.Chat.Abstractions;

public interface IChatAccessService // TODO: implement it some day)
{
    Task<bool> CanAccessChatAsync(Guid userId, Guid chatId, CancellationToken ct);
}