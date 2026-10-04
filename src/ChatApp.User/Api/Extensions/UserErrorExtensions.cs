using ChatApp.User.Features.Common;

namespace ChatApp.User.Api.Extensions;

public static class UserErrorExtensions
{
    public static IResult ToHttpResult(this UserError userError)
    {
        return userError switch
        {
            UserError.UserNotFound => Results.Json(
                data: new { message = "User not found" },
                statusCode: 404
            ),
            _ => Results.Json(data: new { message = "Unknown error" }, statusCode: 500),
        };
    }
}
