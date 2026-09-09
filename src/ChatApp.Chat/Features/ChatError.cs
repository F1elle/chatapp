using ChatApp.Common.Abstractions;

namespace ChatApp.Chat.Features;

public class ChatError : IApiError
{
    public string Code { get; }
    public int HttpStatus { get; }

    private ChatError(string code, int httpStatus)
    {
        Code = code;
        HttpStatus = httpStatus;
    }

    public static readonly ChatError FailedCreatingChat = new("CA-CE01", 500);
    public static readonly ChatError EmptyParticipantList = new("CA-CE11", 400);
}
