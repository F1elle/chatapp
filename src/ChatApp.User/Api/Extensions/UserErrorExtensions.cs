using ChatApp.User.Features.Common;

namespace ChatApp.User.Api.Extensions;

public static class UserErrorExtensions
{
    public static IResult ToHttpResult(this UserError userError)
    {
        return userError switch
        {
            UserError.UserNotFound => Results.Problem(detail: "User not found", statusCode: 404),
            _ => Results.Problem(detail: "Unknown error", statusCode: 500),
        };
    }
}
