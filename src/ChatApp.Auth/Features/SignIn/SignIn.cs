namespace ChatApp.Auth.Features.SignIn;

public sealed record SignInRequest(
    string Email,
    string Password
);

public sealed record SignInResponse(
    string AccessToken,
    string RefreshToken
);