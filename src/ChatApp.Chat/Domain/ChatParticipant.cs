namespace ChatApp.Chat.Domain;

public class ChatParticipant
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }
    public Guid ChatId { get; init; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    public Guid? LastReadMessageId { get; set; }
}