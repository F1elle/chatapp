namespace ChatApp.Auth.Infrastructure.Security;

public record JwtOptions(
    string Secret,
    string Issuer,
    string Audience,
    int ExpirationInMinutes,
    int RefreshTokenExpirationInDays
);