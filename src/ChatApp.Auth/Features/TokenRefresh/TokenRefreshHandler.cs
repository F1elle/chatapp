using ChatApp.Auth.Domain;
using ChatApp.Auth.Infrastructure.Data;
using ChatApp.Auth.Infrastructure.Security;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatApp.Auth.Features.TokenRefresh;

public class TokenRefreshHandler
{
    private readonly AuthDbContext _dbContext;
    private readonly TokenProvider _tokenProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;

    public TokenRefreshHandler(
        AuthDbContext dbContext,
        TokenProvider tokenProvider,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> options)
    {
        _dbContext = dbContext;
        _tokenProvider = tokenProvider;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = options.Value;
    }

    public async Task<Result<TokenRefreshResponse>> Handle(
        TokenRefreshRequest request,
        CancellationToken ct
    )
    {
        var user = await _dbContext.UserAuth
            .Include(ua => ua.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.RefreshToken), ct);

        if (user == null)
            return Result.Failure<TokenRefreshResponse>("Invalid refresh token");

        var refreshToken = user.RefreshTokens.First(t => t.Token == request.RefreshToken);

        if (!refreshToken.IsActive)
        {
            if (refreshToken.IsRevoked)
            {
                RevokeDescendantRefreshTokens(refreshToken, user);
            }

            return Result.Failure<TokenRefreshResponse>("Invalid refresh token");
        }

        var newAccessToken = _tokenProvider.Create(user);
        var newRefreshToken = _tokenProvider.CreateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays);

        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        user.RevokeRefreshToken(request.RefreshToken, newRefreshToken);
        user.AddRefreshToken(newRefreshToken, expiresAt, ipAddress);
        user.RemoveOldRefreshTokens();

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success(new TokenRefreshResponse(
            newAccessToken,
            newRefreshToken
        ));
    }

    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, UserAuth userAuth)
    {
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = userAuth.RefreshTokens
                .FirstOrDefault(t => t.Token == refreshToken.ReplacedByToken);

            if (childToken != null && childToken.IsActive)
            {
                childToken.Revoke();
                RevokeDescendantRefreshTokens(childToken, userAuth);
            }
        }
    }
}