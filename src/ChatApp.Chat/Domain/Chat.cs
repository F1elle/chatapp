using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Domain;

public class Chat
{
    public Guid Id { get; set; }

    public required ChatType Type { get; set; }
    public string? Name { get; set; } = null;
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public Guid? LastMessageId { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public List<ChatParticipant> ChatParticipants { get; set; } = [];
}