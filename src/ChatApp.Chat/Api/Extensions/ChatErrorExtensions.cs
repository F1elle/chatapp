using ChatApp.Chat.Features.Common;

namespace ChatApp.Chat.Api.Extensions;

public static class ChatErrorExtensions
{
    public static IResult ToHttpResult(this ChatError chatError)
    {
        return chatError switch
        {
            ChatError.FailedCreatingChat => Results.Json(
                data: new { message = "Failed creating chat" },
                statusCode: 400
            ), // TODO: find proper code
            ChatError.EmptyParticipantList => Results.Json(
                data: new { message = "No participants selected" },
                statusCode: 400
            ), // ("No participants selected", 400),
            ChatError.ChatNotFound => Results.Json(
                data: new { message = "Chat not found" },
                statusCode: 404
            ), // ("Chat not found", 404),
            ChatError.NotGroupChat => Results.Json(
                data: new { message = "Not a group chat" },
                statusCode: 400
            ), // ("Not a group chat", 400),
            ChatError.InvalidMessage => Results.Json(
                data: new { message = "Invalid message format" },
                statusCode: 400
            ), // ("Invalid message format", 400),
            _ => Results.Json(data: new { message = "Unknown error" }, statusCode: 500),
        };
    }
}
