namespace ChatApp.Auth.Domain;

public class UserAuthInfo
{
    public Guid Id { get; init; }
    public string UserName { get; set; } = string.Empty;
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}