namespace ChatApp.Chat.Domain;

public class UserProfileSnapshot
{
    // PK, Corresponds User.Id from User service
    public Guid UserId { get; init; }

    public string DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime SyncedAt { get; private set; }

    public void UpdateFrom(string displayName, string? avatarUrl)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        SyncedAt = DateTime.UtcNow;
    }
}
