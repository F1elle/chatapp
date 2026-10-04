using ChatApp.Chat.Features.Common;

namespace ChatApp.Chat.Api.Extensions;

public static class ChatErrorExtensions
{
    public static IResult ToHttpResult(this ChatError chatError)
    {
        return chatError switch
        {
            ChatError.FailedCreatingChat => Results.Problem(
                detail: "Failed creating chat",
                statusCode: 400
            ),
            ChatError.EmptyParticipantList => Results.Problem(
                detail: "No participants selected",
                statusCode: 400
            ),
            ChatError.ChatNotFound => Results.Problem(detail: "Chat not found", statusCode: 404),
            ChatError.NotGroupChat => Results.Problem(detail: "Not a group chat", statusCode: 400),
            ChatError.InvalidMessage => Results.Problem(
                detail: "Invalid message format",
                statusCode: 400
            ),
            _ => Results.Problem(detail: "Unknown error", statusCode: 500),
        };
    }
}
