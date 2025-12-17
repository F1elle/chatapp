namespace ChatApp.Chat.Features.Abstractions;

public interface IChatAccessService // TODO: implement it some day)
{
    Task<bool> CanAccessChatAsync(Guid userId, Guid chatId, CancellationToken ct);
}