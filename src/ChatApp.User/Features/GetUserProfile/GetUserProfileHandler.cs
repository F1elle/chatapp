using ChatApp.Common.Abstractions;
using ChatApp.User.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Features.GetUserProfile;

public class GetUserProfileHandler
    : IHandler<GetUserProfileRequest, Result<GetUserProfileResponse, UserError>>
{
    private readonly UserDbContext _dbContext;

    public GetUserProfileHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetUserProfileResponse, UserError>> Handle(
        GetUserProfileRequest request,
        CancellationToken ct
    )
    {
        var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(
            up => up.Id == request.Id,
            ct
        );

        return userProfile == null
            ? UserError.UserNotFound
            : new GetUserProfileResponse(
                userProfile.Id,
                userProfile.DisplayName,
                userProfile.Email,
                userProfile.Bio,
                userProfile.ProfilePictureId,
                userProfile.UserTag,
                userProfile.CreatedAt
            );
    }
}
