using ChatApp.Chat.Features.Common.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Chat.Api.Hubs;

[RegisterSingleton]
public class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub, IChatClient> _hub;
    private readonly ILogger<ChatNotifier> _logger;

    public ChatNotifier(IHubContext<ChatHub, IChatClient> hub, ILogger<ChatNotifier> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task NotifyNewMessage(Guid chatId, MessageDto message, CancellationToken ct)
    {
        try
        {
            await _hub.Clients.Group(SignalRGroups.ChatGroup(chatId)).ReceiveMessage(message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Broadcast failed for message {MessageId} in chat {ChatId}",
                message.Id,
                chatId
            );
        }
    }
}
