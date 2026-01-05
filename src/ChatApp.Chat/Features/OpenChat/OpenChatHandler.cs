using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Features.Abstractions;
using CSharpFunctionalExtensions;

namespace ChatApp.Chat.Features.OpenChat;

public class OpenChatHandler : IHandler<OpenChatRequest, Result<OpenChatResponse>>
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

    public async Task<Result<OpenChatResponse>> Handle(OpenChatRequest request, CancellationToken ct)
    {
        var participantId = await _chatAccess.GetParticipantIdAsync(
            request.UserId,
            request.ChatId,
            ct);
        
        if (participantId == null)
            return Result.Failure<OpenChatResponse>("Access denied");

        await _chatPresence.MarkActiveAsync(request.ChatId, (Guid)participantId, ct);

        // TODO: maybe return active users

        return new OpenChatResponse((Guid)participantId);
    }
}