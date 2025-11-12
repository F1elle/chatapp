namespace ChatApp.User.Features.UserProfile.CreateUserProfile;

public sealed record CreateUserProfileRequest(
    Guid Id,
    string Email,
    string DisplayName,
    DateTime CreatedAt);