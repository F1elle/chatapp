using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Features.Abstractions;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.ActivateChat;

public class ActivateChatHandler : IHandler<ActivateChatRequest, Result<ActivateChatResponse>>
{
    private readonly IChatAccessService _chatAccess;
    private readonly IChatPresenceService _chatPresence;

    public ActivateChatHandler(IChatAccessService chatAccess, IChatPresenceService chatPresence)
    {
        _chatAccess = chatAccess;
        _chatPresence = chatPresence;
    }

    public async Task<Result<ActivateChatResponse>> Handle(
        ActivateChatRequest request,
        CancellationToken ct
    )
    {
        var participantId = await _chatAccess.GetParticipantIdAsync(
            request.UserId,
            request.ChatId,
            ct
        );

        if (participantId == null)
            return Result.Failure<ActivateChatResponse>("Access denied");

        await _chatPresence.MarkActiveAsync(request.ChatId, (Guid)participantId, ct);

        // TODO: maybe return active users

        return new ActivateChatResponse((Guid)participantId);
    }
}
