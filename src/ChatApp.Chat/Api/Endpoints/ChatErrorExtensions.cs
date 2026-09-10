using ChatApp.Chat.Features.Common;
using ChatApp.Common;

namespace ChatApp.Chat.Api.Endpoints;

public static class ChatErrorExtensions
{
    public static ApiError ToApiError(this ChatError error) =>
        error switch
        {
            ChatError.FailedCreatingChat => new ApiError("CA-CE01", 500),
            ChatError.EmptyParticipantList => new ApiError("CA-CE11", 400),
            _ => new ApiError("CA-CE00", 500),
        };
}
