using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Domain;

public class Message
{
    public Guid Id { get; init; }
    public ChatParticipant ParticipantSender { get; init; } = null!;
    public required Guid ParticipantSenderId { get; init; }
    public required Guid ChatId { get; init; }
    public required DateTime SentAt { get; init; } 
    public DateTime? EditedAt { get; set; } = null;
    public string? Content { get; set; } 
    public required MessageType Type { get; init; } 

    public List<Guid> AttachmentIds { get; init; } = [];


    public List<MessageSeen> SeenByParticipants { get; set; } = [];


    public bool IsEdited => EditedAt != null;
    public bool IsRead => SeenByParticipants.Any();
    public int SeenCount => SeenByParticipants.Count;


    private Message() {}

    public static Message CreateTextMessage(Guid senderId, Guid chatId, string content)
    {
        return new Message()
        {
            ParticipantSenderId = senderId,
            ChatId = chatId,
            SentAt = DateTime.UtcNow,
            Content = content,
            Type = MessageType.Text
        };
    }

    public static Message CreateMessageWithAttachments(Guid senderId, Guid chatId, string? content, List<Guid> attachmentIds)
    {
        return new Message()
        {
            ParticipantSenderId = senderId,
            ChatId = chatId,
            SentAt = DateTime.UtcNow,
            Content = content,
            Type = MessageType.WithMediaAttachments
        };
    }

    public static Message CreateSystemMessage(Guid chatId, string content)
    {
        return new Message()
        {
            ParticipantSenderId = Guid.Empty, // TODO: maybe some predefined value
            ChatId = chatId,
            SentAt = DateTime.UtcNow,
            Content = content,
            Type = MessageType.System
        };
    }
}