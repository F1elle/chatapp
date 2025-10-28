namespace ChatApp.Auth.Features.SignUp;

public record SignUpRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);