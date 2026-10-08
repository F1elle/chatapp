using ChatApp.Auth.Domain;
using ChatApp.Auth.Features.Common;
using ChatApp.Auth.Infrastructure.Data;
using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Common.Abstractions;
using ChatApp.Common.Infrastructure.Messaging.Events;
using ChatApp.Common.Infrastructure.Outbox;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Features.SignUp;

[RegisterScoped]
public class SignUpHandler : IHandler<SignUpRequest, Result<SignUpResponse, AuthError>>
{
    private readonly AuthDbContext _dbContext;
    private readonly PasswordHasher _passwordHasher;
    private readonly IOutboxWriter _outboxWriter;

    public SignUpHandler(
        AuthDbContext dbContext,
        PasswordHasher passwordHasher,
        IOutboxWriter outboxWriter
    )
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _outboxWriter = outboxWriter;
    }

    public async Task<Result<SignUpResponse, AuthError>> Handle(
        SignUpRequest request,
        CancellationToken ct
    )
    {
        var userAuth = _dbContext.UserAuth;

        var existingUser = await userAuth.FirstOrDefaultAsync(ua => ua.Email == request.Email, ct);

        if (existingUser != null)
            return AuthError.EmailIsTaken;

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var createdUserAuth = new UserAuth { Email = request.Email, PasswordHash = passwordHash };

        userAuth.Add(createdUserAuth);

        _outboxWriter.Enqueue(
            new UserSignedUpEvent(
                UserId: createdUserAuth.Id,
                Email: request.Email,
                DisplayName: request.DisplayName ?? request.Email.Split('@')[0],
                SignedUpAt: createdUserAuth.CreatedAt
            )
        );

        await _dbContext.SaveChangesAsync(ct);

        return new SignUpResponse(createdUserAuth.Id);
    }
}
