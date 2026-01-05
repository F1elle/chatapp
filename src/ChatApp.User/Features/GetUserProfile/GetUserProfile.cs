using System;

namespace ChatApp.User.Features.GetUserProfile;

public sealed record GetUserProfileRequest(Guid Id);

public sealed record GetUserProfileResponse(
    Guid Id,
    string DisplayName,
    string Email,
    string? Bio,
    Guid? ProfilePictureId,
    string UserTag,
    DateTime CreatedAt 
);

