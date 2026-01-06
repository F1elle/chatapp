using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Domain;

public class Chat
{
    public Guid Id { get; init; }

    public required ChatType Type { get; init; }
    public string? Name { get; set; } = null;
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; init; } 
    public Message? LastMessage { get; set; }
    public Guid? LastMessageId { get; set; } 
    public DateTime? LastMessageAt { get; set; }

    public List<ChatParticipant> ChatParticipants { get; set; } = [];
    public List<Message> Messages { get; set; } = [];



    private Chat() {}

    public static Chat CreatePrivateChat()
    {
        return new Chat() 
        {
            Type = ChatType.Direct, 
            Name = null,
            CreatedBy = null,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public static Chat CreateGroupChat(Guid createdBy, string name)
    {
        return new Chat()
        {
            Type = ChatType.Group,
            Name = name,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void UpdateLastMessage(Message message)
    {
        LastMessageId = message.Id;
        LastMessageAt = message.SentAt;
        LastMessage = message; 
    }

    // TODO: add chat participants, return result
}