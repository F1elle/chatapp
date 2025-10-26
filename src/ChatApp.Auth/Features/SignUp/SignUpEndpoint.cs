using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Auth.Infrastructure.Data;

namespace ChatApp.Auth.Features.SignUp;

public static class SignUpEndpoint
{
    public static IEndpointRouteBuilder MapSignUpEndpoint(this RouteGroupBuilder route)
    {
        route.MapPost("/signup", SignUp);

        return route;
    }

    private static async Task<IResult> SignUp(
            SignUpRequest request,
            SignupHandler handler,
            AuthDbContext dbContext,
            IPasswordHasher passwordHasher,
            CancellationToken ct)
    {
        var result = await handler.HandleSignUp(request, dbContext, passwordHasher, ct);


        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest();
    }
}
