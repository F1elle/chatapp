using ChatApp.Auth.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Features.TokenRevoke;

public class TokenRevokeHandler
    : IHandler<TokenRevokeRequest, Result<TokenRevokeResponse, AuthError>>
{
    private readonly AuthDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenRevokeHandler(AuthDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<TokenRevokeResponse, AuthError>> Handle(
        TokenRevokeRequest request,
        CancellationToken ct
    )
    {
        var user = await _dbContext
            .UserAuth.Include(ua => ua.RefreshTokens)
            .FirstOrDefaultAsync(
                ua => ua.RefreshTokens.Any(t => t.Token == request.RefreshToken),
                ct
            );

        if (user == null)
            return AuthError.InvalidRefreshToken;

        var refreshToken = user.RefreshTokens.First(t => t.Token == request.RefreshToken);

        if (!refreshToken.IsActive)
            return AuthError.InvalidRefreshToken;

        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        refreshToken.Revoke(ipAddress: ipAddress);

        await _dbContext.SaveChangesAsync(ct);

        return new TokenRevokeResponse();
    }
}
