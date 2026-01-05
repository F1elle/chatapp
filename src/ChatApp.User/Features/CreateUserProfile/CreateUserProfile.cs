namespace ChatApp.User.Features.CreateUserProfile;

public sealed record CreateUserProfileRequest(
    Guid Id,
    string Email,
    string DisplayName,
    DateTime CreatedAt);

public sealed record CreateUserProfileResponse();