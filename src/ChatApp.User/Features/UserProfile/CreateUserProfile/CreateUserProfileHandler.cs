using ChatApp.User.Infrastructure.Data;
using CSharpFunctionalExtensions;

namespace ChatApp.User.Features.UserProfile.CreateUserProfile;

public class CreateUserProfileHandler
{
    private readonly UserDbContext _dbContext;

    public CreateUserProfileHandler(
        UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        CreateUserProfileRequest request,
        CancellationToken ct)
    {
        _dbContext.UserProfiles.Add(new Domain.UserProfile
        {
            Id = request.Id,
            Email = request.Email,
            DisplayName = request.DisplayName,
            CreatedAt = request.CreatedAt,
            UserTag = Guid.NewGuid().ToString()
        });

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}