using ChatApp.Auth.Infrastructure.Data;
using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatApp.Auth.Features.SignIn;

public class SignInHandler : IHandler<SignInRequest, Result<SignInResponse, AuthError>>
{
    private readonly AuthDbContext _dbContext;
    private readonly PasswordHasher _passwordHasher;
    private readonly TokenProvider _tokenProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;

    public SignInHandler(
        AuthDbContext dbContext,
        PasswordHasher passwordHasher,
        TokenProvider tokenProvider,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<SignInResponse, AuthError>> Handle(
        SignInRequest request,
        CancellationToken ct
    )
    {
        var user = await _dbContext
            .UserAuth.Include(ua => ua.RefreshTokens)
            .FirstOrDefaultAsync(ua => ua.Email == request.Email);

        if (user == null)
            return AuthError.InvalidCredentials;

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return AuthError.InvalidCredentials;

        var accessToken = _tokenProvider.Create(user);
        var refreshToken = _tokenProvider.CreateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays);

        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        user.AddRefreshToken(refreshToken, expiresAt, ipAddress);

        user.RemoveOldRefreshTokens();

        await _dbContext.SaveChangesAsync(ct);

        return new SignInResponse(accessToken, refreshToken);
    }
}
