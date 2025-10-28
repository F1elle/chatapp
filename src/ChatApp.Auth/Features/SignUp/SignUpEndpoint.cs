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
            SignUpHandler handler,
            CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);


        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest();
    }
}
