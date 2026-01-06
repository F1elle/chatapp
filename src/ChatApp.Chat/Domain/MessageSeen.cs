namespace ChatApp.Chat.Domain;

public class MessageSeen
{
    public Guid Id { get; init; }
    public Guid MessageId { get; set; }
    public Guid ParticipantId { get; set; }
    public DateTime SeenAt { get; set; } = DateTime.UtcNow;

    private MessageSeen() {}

    public MessageSeen(Guid messageId, Guid participantId)
    {
        MessageId = messageId;
        ParticipantId = participantId;
        SeenAt = DateTime.UtcNow;
    }
}

