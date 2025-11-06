using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Domain;

public class Message
{
    public Guid Id { get; init; }
    public required Guid SenderId { get; set; }
    public DateTime SentAt { get; init; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; } = null;
    public required string Content { get; set; }
    public required MessageType Type { get; set; } = MessageType.Text;

    public List<Guid> AttachmentIds { get; set; } = [];

    public Guid ReplyToMessageId { get; set; }

    public bool IsEdited => EditedAt != null;
}