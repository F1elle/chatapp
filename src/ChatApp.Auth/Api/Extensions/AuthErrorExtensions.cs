using ChatApp.Auth.Features.Common;

namespace ChatApp.Auth.Api.Extensions;

public static class AuthErrorExtensions
{
    public static IResult ToHttpResult(this AuthError authError)
    {
        return authError switch
        {
            AuthError.InvalidCredentials => Results.Json(
                data: new { message = "Invalid credentials" },
                statusCode: 401
            ),
            AuthError.EmailIsTaken => Results.Json(
                data: new { message = "This email is already taken" },
                statusCode: 400
            ),
            AuthError.InvalidRefreshToken => Results.Json(
                data: new { message = "Refresh token is not valid" },
                statusCode: 403
            ),
            _ => Results.Json(data: new { message = "Unknown error" }, statusCode: 500),
        };
    }
}
