using ChatApp.Auth.Infrastructure.Data;
using ChatApp.Auth.Infrastructure.Security;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatApp.Auth.Features.SignIn;

public class SignInHandler
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
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<SignInResponse>> Handle(SignInRequest request, CancellationToken ct)
    {
        var user = await _dbContext.UserAuth
            .Include(ua => ua.RefreshTokens)
            .FirstOrDefaultAsync(ua => ua.Email == request.Email);

        if (user == null)
            return Result.Failure<SignInResponse>("Invalid email or password");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result.Failure<SignInResponse>("Invalid email or password");

        var accessToken = _tokenProvider.Create(user);
        var refreshToken = _tokenProvider.CreateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays);

        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        user.AddRefreshToken(refreshToken, expiresAt, ipAddress);

        user.RemoveOldRefreshTokens();

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success(new SignInResponse(accessToken, refreshToken));
    }
}