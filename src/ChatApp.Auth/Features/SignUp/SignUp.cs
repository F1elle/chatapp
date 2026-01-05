namespace ChatApp.Auth.Features.SignUp;

public sealed record SignUpRequest(
    string Email,
    string Password,
    string? DisplayName
);

public sealed record SignUpResponse(Guid UserId);