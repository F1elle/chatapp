namespace ChatApp.Chat.Domain;

// for future feature
public class MessageSeen
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid MessageId { get; init; }
    public Guid ParticipantId { get; init; }
    public DateTime SeenAt { get; init; } = DateTime.UtcNow;

    private MessageSeen() { }

    public MessageSeen(Guid messageId, Guid participantId)
    {
        MessageId = messageId;
        ParticipantId = participantId;
        SeenAt = DateTime.UtcNow;
    }
}
