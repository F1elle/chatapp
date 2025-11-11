namespace ChatApp.Chat.Domain;

public class ChatParticipant
{
    public Guid Id { get; init; }

    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public Guid? LastReadMessageId { get; set; }
}