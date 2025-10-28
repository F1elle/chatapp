namespace ChatApp.Auth.Domain;

public class UserAuth
{
    public Guid Id { get; init; }
    public string UserName { get; set; } = string.Empty;
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    
    public RefreshToken AddRefreshToken(string token, DateTime expiresAt, string? ipAddress = null)
    {
        var refreshToken = new RefreshToken(Id, token, expiresAt, ipAddress);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }
    
    public void RevokeRefreshToken(string token, string? replacedByToken = null)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null) return;
        
        refreshToken.Revoke(replacedByToken);
    }
    
    public void RemoveOldRefreshTokens(int ttl = 30)
    {
        _refreshTokens.RemoveAll(t => 
            !t.IsActive && 
            t.CreatedAt.AddDays(ttl) <= DateTime.UtcNow);
    }
}