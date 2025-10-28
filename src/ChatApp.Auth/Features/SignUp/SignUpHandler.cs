using ChatApp.Auth.Domain;
using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Auth.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Features.SignUp;

public class SignUpHandler
{
    private readonly AuthDbContext _dbContext;
    private readonly PasswordHasher _passwordHasher;

    public SignUpHandler(
        AuthDbContext dbContext,
        PasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(
        SignUpRequest request,
        CancellationToken ct)
    {
        var userAuth = _dbContext.UserAuth;

        var existingUser = await userAuth.FirstOrDefaultAsync(ua => ua.Email == request.Email, ct);

        if (existingUser == null)
            return Result.Failure("User with such email already exists");


        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var createdUserAuth = new UserAuth
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };

        userAuth.Add(createdUserAuth);

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}