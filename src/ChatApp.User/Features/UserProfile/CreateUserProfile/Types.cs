namespace ChatApp.User.Features.UserProfile.CreateUserProfile;

public record CreateUserProfileRequest(
    Guid Id,
    string Email,
    string DisplayName,
    DateTime CreatedAt);