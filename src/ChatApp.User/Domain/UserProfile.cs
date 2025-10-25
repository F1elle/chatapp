namespace ChatApp.User.Domain;


public class UserProfile
{
    public Guid Id { get; init; }
    public string? FirstName { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? Bio { get; set; } = string.Empty;
    public Guid? ProfilePictureId { get; set; } = null;
    public required string UserName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}