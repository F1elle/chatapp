using ChatApp.Common.Abstractions;

namespace ChatApp.User.Features;

public class UserError : IApiError
{
    public string Code { get; }
    public int HttpStatus { get; }

    private UserError(string code, int httpStatus)
    {
        Code = code;
        HttpStatus = httpStatus;
    }

    public static readonly UserError UserNotFound = new("CA-UE01", 404);
}
