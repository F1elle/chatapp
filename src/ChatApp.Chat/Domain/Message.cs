using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Domain;

public class Message
{
    public Guid Id { get; init; }
    public ChatParticipant? ParticipantSender { get; set; } 
    public required Guid ParticipantSenderId { get; set; }
    public required Guid ChatId { get; set; }
    public DateTime SentAt { get; init; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; } = null;
    public string? Content { get; set; } 
    public required MessageType Type { get; set; } = MessageType.Text;

    public List<Guid> AttachmentIds { get; set; } = [];

    public Message? ReplyToMessage { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public List<Message> Replies { get; set; } = [];

    public List<MessageSeen> SeenByParticipants { get; set; } = [];

    public bool IsEdited => EditedAt != null;
    public bool IsRead => SeenByParticipants.Any();
    public int RepliesCount => Replies.Count;
    public int SeenCount => SeenByParticipants.Count;
}