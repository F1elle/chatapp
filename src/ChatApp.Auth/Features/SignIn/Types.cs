namespace ChatApp.Auth.Features.SignIn;

public record SignInRequest(
    string Login,
    string Password
);