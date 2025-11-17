using ChatApp.Chat.Features.Chat.Abstractions;

namespace ChatApp.Chat.Features.Chat.CloseChat;

public class CloseChatHandler
{
    private readonly IChatPresenceService _chatPresence;
    public CloseChatHandler(
        IChatPresenceService chatPresence)
    {
        _chatPresence = chatPresence;
    }

    public async Task Handle(CloseChatRequest request, CancellationToken ct)
    {
        await _chatPresence.MarkInactiveAsync(request.ChatId, request.UserId, ct);
    }
}