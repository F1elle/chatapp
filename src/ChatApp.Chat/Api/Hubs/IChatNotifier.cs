using ChatApp.Chat.Api.Contracts;

namespace ChatApp.Chat.Api.Hubs;

public interface IChatNotifier
{
    Task NotifyNewMessage(Guid chatId, MessageResponse message, CancellationToken ct);
}
