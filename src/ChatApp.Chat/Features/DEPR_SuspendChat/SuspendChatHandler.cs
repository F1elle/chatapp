using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Features.Abstractions;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.SuspendChat;

public class SuspendChatHandler : IHandler<SuspendChatRequest, Result<SuspendChatResponse>>
{
    private readonly IChatPresenceService _chatPresence;

    public SuspendChatHandler(IChatPresenceService chatPresence)
    {
        _chatPresence = chatPresence;
    }

    public async Task<Result<SuspendChatResponse>> Handle(
        SuspendChatRequest request,
        CancellationToken ct
    )
    {
        await _chatPresence.MarkInactiveAsync(request.ChatId, request.UserId, ct);
        return new SuspendChatResponse();
    }
}
