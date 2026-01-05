using ChatApp.Auth.Domain;
using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Auth.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Rebus.Bus;
using ChatApp.Common.Infrastructure.Messaging.Events;
using ChatApp.Auth.Common.Abstractions;

namespace ChatApp.Auth.Features.SignUp;

public class SignUpHandler : IHandler<SignUpRequest, Result<SignUpResponse>>
{
    private readonly AuthDbContext _dbContext;
    private readonly PasswordHasher _passwordHasher;
    private readonly IBus _bus;

    public SignUpHandler(
        AuthDbContext dbContext,
        PasswordHasher passwordHasher,
        IBus bus)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _bus = bus;
    }

    public async Task<Result<SignUpResponse>> Handle(
        SignUpRequest request,
        CancellationToken ct)
    {
        var userAuth = _dbContext.UserAuth;

        var existingUser = await userAuth.FirstOrDefaultAsync(ua => ua.Email == request.Email, ct);

        if (existingUser != null)
            return Result.Failure<SignUpResponse>("User with such email already exists");


        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var createdUserAuth = new UserAuth
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };

        userAuth.Add(createdUserAuth);

        await _dbContext.SaveChangesAsync(ct);

        await _bus.Send(new UserSignedUpEvent(
            UserId: createdUserAuth.Id,
            Email: request.Email,
            DisplayName: request.DisplayName ?? request.Email.Split('@')[0],
            SignedUpAt: createdUserAuth.CreatedAt
        ));

        return new SignUpResponse(createdUserAuth.Id);
    }
}