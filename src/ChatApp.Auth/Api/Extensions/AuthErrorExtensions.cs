using ChatApp.Auth.Features.Common;

namespace ChatApp.Auth.Api.Extensions;

public static class AuthErrorExtensions
{
    public static IResult ToHttpResult(this AuthError authError)
    {
        return authError switch
        {
            AuthError.InvalidCredentials => Results.Problem(
                detail: "Invalid credentials",
                statusCode: 401
            ),
            AuthError.EmailIsTaken => Results.Problem(
                detail: "This email is already taken",
                statusCode: 400
            ),
            AuthError.InvalidRefreshToken => Results.Problem(
                detail: "Refresh token is not valid",
                statusCode: 403
            ),
            _ => Results.Problem(detail: "Unknown error", statusCode: 500),
        };
    }
}
