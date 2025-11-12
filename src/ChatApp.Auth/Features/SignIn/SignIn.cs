namespace ChatApp.Auth.Features.SignIn;

public record SignInRequest(
    string Email,
    string Password
);

public record SignInResponse(
    string AccessToken,
    string RefreshToken
);