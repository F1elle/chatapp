using ChatApp.Auth.Common.Abstractions;
using ChatApp.Auth.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Features.TokenRevoke;

public class TokenRevokeHandler : IHandler<TokenRevokeRequest, Result<TokenRevokeResponse>>
{
    private readonly AuthDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenRevokeHandler(
        AuthDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<TokenRevokeResponse>> Handle(TokenRevokeRequest request, CancellationToken ct)
    {
        var user = await _dbContext.UserAuth
            .Include(ua => ua.RefreshTokens)
            .FirstOrDefaultAsync(ua => ua.RefreshTokens.Any(t => t.Token == request.RefreshToken), ct);

        if (user == null)
            return Result.Failure<TokenRevokeResponse>("Invalid refresh token");

        var refreshToken = user.RefreshTokens.First(t => t.Token == request.RefreshToken);

        if (!refreshToken.IsActive)
            return Result.Failure<TokenRevokeResponse>("Invalid refresh token");

        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        refreshToken.Revoke(ipAddress: ipAddress);

        await _dbContext.SaveChangesAsync(ct);

        return new TokenRevokeResponse();
    }
}