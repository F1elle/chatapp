using ChatApp.User.Common.Abstractions;
using ChatApp.User.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Features.GetUserProfile;

public class GetUserProfileHandler : IHandler<GetUserProfileRequest, Result<GetUserProfileResponse>>
{
    private readonly UserDbContext _dbContext;

    public GetUserProfileHandler(
        UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetUserProfileResponse>> Handle(GetUserProfileRequest request, CancellationToken ct)
    {
        var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(up => up.Id == request.Id, ct);

        return userProfile == null
            ? Result.Failure<GetUserProfileResponse>("User not found") 
            : new GetUserProfileResponse(userProfile.Id, 
                userProfile.DisplayName, 
                userProfile.Email,
                userProfile.Bio, 
                userProfile.ProfilePictureId, 
                userProfile.UserTag, 
                userProfile.CreatedAt);
    }
}