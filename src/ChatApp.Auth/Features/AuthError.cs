using ChatApp.Common.Abstractions;

namespace ChatApp.Auth.Features;

public class AuthError : IApiError
{
    public string Code { get; }
    public int HttpStatus { get; }

    private AuthError(string code, int httpStatus)
    {
        Code = code;
        HttpStatus = httpStatus;
    }

    public static readonly AuthError InvalidCredentials = new("CA-AE01", 401);
    public static readonly AuthError EmailIsTaken = new("CA-AE02", 400);
    public static readonly AuthError InvalidRefreshToken = new("CA-AE03", 400);
}
