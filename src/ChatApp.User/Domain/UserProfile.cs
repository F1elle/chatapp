namespace ChatApp.User.Domain;


public class UserProfile
{
    public Guid Id { get; init; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public string? Bio { get; set; } = string.Empty;
    public Guid? ProfilePictureId { get; set; } = null;
    public required string UserTag { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}