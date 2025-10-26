using Microsoft.AspNetCore.Http.HttpResults;

namespace ChatApp.Auth.Features.Signup;

public static class AuthEndpoints
{
    public static RouteGroupBuilder RegisterAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/signup", Signup);

        return group;
    }
    private static async Task<Ok> Signup(SignupRequest request)
    {
        return TypedResults.Ok();
    }

    private record SignupRequest(
        string Email,
        string Password
    );
}

