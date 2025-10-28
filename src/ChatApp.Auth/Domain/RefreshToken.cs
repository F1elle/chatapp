namespace ChatApp.Auth.Domain;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserAuthId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? CreatedByIp { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    public RefreshToken(Guid userAuthId, string token, DateTime expiresAt, string? ipAddress)
    {
        Id = Guid.NewGuid();
        UserAuthId = userAuthId;
        Token = token;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        CreatedByIp = ipAddress;
    }

    public void Revoke(string? replacedByToken = null, string? ipAddress = null)
    {
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = ipAddress;
        ReplacedByToken = replacedByToken;
    }

    private RefreshToken() { }
}