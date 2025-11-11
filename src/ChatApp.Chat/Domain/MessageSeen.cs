namespace ChatApp.Chat.Domain;

public class MessageSeen
{
    public Guid MessageId { get; set; }
    public Guid ParticipantId { get; set; }
    public DateTime SeenAt { get; set; } = DateTime.UtcNow;
}
