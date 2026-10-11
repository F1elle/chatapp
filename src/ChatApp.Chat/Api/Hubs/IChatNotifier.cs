using ChatApp.Chat.Features.Common.Contracts;

namespace ChatApp.Chat.Api.Hubs;

public interface IChatNotifier
{
    Task NotifyNewMessage(Guid chatId, MessageDto message, CancellationToken ct);
}
