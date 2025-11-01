using ChatApp.User.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Features.UserProfile.GetUserProfile;

public class GetUserProfileHandler
{
    private readonly UserDbContext _dbContext;

    public GetUserProfileHandler(
        UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Domain.UserProfile>> Handle(Guid id, CancellationToken ct)
    {
        var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(up => up.Id == id, ct);

        return userProfile == null
            ? Result.Failure<Domain.UserProfile>("User not found")
            : Result.Success(userProfile);
    }
}