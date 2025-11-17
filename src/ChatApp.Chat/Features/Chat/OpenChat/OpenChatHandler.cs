using ChatApp.Chat.Features.Chat.Abstractions;

namespace ChatApp.Chat.Features.Chat.OpenChat;

public class OpenChatHandler
{
    private readonly IChatAccessService _chatAccess;
    private readonly IChatPresenceService _chatPresence;
    public OpenChatHandler(
        IChatAccessService chatAccess,
        IChatPresenceService chatPresence)
    {
        _chatAccess = chatAccess;
        _chatPresence = chatPresence;
    }

    public async Task<OpenChatResponse> Handle(OpenChatRequest request, CancellationToken ct)
    {
        var hasAccess = await _chatAccess.CanAccessChatAsync(
            request.UserId,
            request.ChatId,
            ct);
        
        if (!hasAccess)
            return new OpenChatResponse(false);

        await _chatPresence.MarkActiveAsync(request.ChatId, request.UserId, ct);

        // TODO: maybe return active users

        return new OpenChatResponse(true);
    }
}