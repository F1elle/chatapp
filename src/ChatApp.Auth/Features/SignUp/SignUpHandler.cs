using ChatApp.Auth.Domain;
using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Auth.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Features.SignUp;

public class SignupHandler
{
    public async Task<Result> HandleSignUp(
        SignUpRequest request,
        AuthDbContext dbContext,
        IPasswordHasher passwordHasher,
        CancellationToken ct)
    {
        var userAuth = dbContext.UserAuth;

        var existingUser = await userAuth.FirstOrDefaultAsync(ua => ua.Email == request.Email, ct);

        if (existingUser == null)
            return Result.Failure("User with such email already exists");


        var passwordHash = passwordHasher.HashPassword(request.Password);

        var createdUserAuth = new UserAuthInfo
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };

        userAuth.Add(createdUserAuth);

        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}