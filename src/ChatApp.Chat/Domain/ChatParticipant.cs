namespace ChatApp.Chat.Domain;

public class ChatParticipant
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }
    public Guid ChatId { get; init; }
    public DateTime JoinedAt { get; set; } 
    public Guid? LastReadMessageId { get; set; }

    private ChatParticipant() {}


    public ChatParticipant(Guid userId, Guid chatId)
    {
        UserId = userId;
        ChatId = chatId;
        JoinedAt = DateTime.UtcNow;
    }

    public void UpdateLastReadMessageId(Guid messageId)
    {
        LastReadMessageId = messageId;
    }
}