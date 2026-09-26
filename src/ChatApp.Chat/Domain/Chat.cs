using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Domain;

public class Chat
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public required ChatType Type { get; init; }

    // Either Chat name or UserSnapshot name depending on the type
    public string? Name { get; private set; } = null;

    public Guid? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public Message? LastMessage { get; private set; }
    public Guid? LastMessageId { get; private set; }

    // For ordering in chat list
    public DateTime LastUpdateAt { get; private set; }

    private readonly List<ChatParticipant> _participants = [];
    public IReadOnlyList<ChatParticipant> Participants => _participants;

    [Obsolete("No-no, remove once removed everywhere")]
    public List<Message> Messages { get; set; } = [];

    // For EFCore
    private Chat() { }

    public static Chat CreatePrivateChat()
    {
        return new Chat()
        {
            Type = ChatType.Direct,
            Name = null,
            CreatedBy = null,
            CreatedAt = DateTime.UtcNow,
            LastUpdateAt = DateTime.UtcNow,
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
            LastUpdateAt = DateTime.UtcNow,
        };
    }

    public void UpdateLastMessage(Message message)
    {
        LastMessageId = message.Id;
        LastUpdateAt = message.SentAt;
        LastMessage = message;
    }

    // TODO: add chat participants, return result
}
