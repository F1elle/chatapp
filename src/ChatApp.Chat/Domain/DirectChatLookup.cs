namespace ChatApp.Chat.Domain;

public class DirectChatLookup
{
    public Guid ChatId { get; init; }
    public Guid UserIdLow { get; init; }
    public Guid UserIdHigh { get; init; }
}
