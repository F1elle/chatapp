namespace ChatApp.Chat.Domain;

// Relation entity between user and chat
public class ChatParticipant
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public required Guid UserId { get; init; }
    public UserProfileSnapshot User { get; init; }
    public required Guid ChatId { get; init; }
    public Chat Chat { get; init; }
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;

    // For EFCore
    private ChatParticipant() { }

    public ChatParticipant(Guid userId, Guid chatId)
    {
        UserId = userId;
        ChatId = chatId;
    }
}
