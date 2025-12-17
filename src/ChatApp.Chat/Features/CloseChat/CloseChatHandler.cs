using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Features.Abstractions;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.CloseChat;

public class CloseChatHandler : IHandler<CloseChatRequest, Result<CloseChatResponse>>
{
    private readonly IChatPresenceService _chatPresence;
    public CloseChatHandler(
        IChatPresenceService chatPresence)
    {
        _chatPresence = chatPresence;
    }

    public async Task<Result<CloseChatResponse>> Handle(CloseChatRequest request, CancellationToken ct)
    {
        await _chatPresence.MarkInactiveAsync(request.ChatId, request.UserId, ct);
        return new CloseChatResponse();
    }
}