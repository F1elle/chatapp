using ChatApp.Auth.Features.SignIn;
using ChatApp.Auth.Features.SignUp;
using ChatApp.Auth.Features.TokenRefresh;
using ChatApp.Auth.Features.TokenRevoke;

namespace ChatApp.Auth.Features;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication");


        group.MapPost("/signup", SignUpRoute)
            .WithName("SignUp")
            .WithSummary("Register new user")
            .AllowAnonymous();

        group.MapPost("/signin", SignInRoute)
            .WithName("SignIn")
            .WithSummary("Authenticate user and get tokens");

        group.MapPost("/refreshtoken", TokenRefreshRoute)
            .WithName("RefreshToken")
            .AllowAnonymous();

        group.MapPost("/revoketoken", TokenRevokeRoute)
            .WithName("RevokeToken")
            .RequireAuthorization();

        return app;
    }


    public static async Task<IResult> SignUpRoute(
            SignUpRequest request,
            SignUpHandler handler,
            CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);


        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> SignInRoute(
            SignInRequest request,
            SignInHandler handler,
            CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> TokenRefreshRoute(
        TokenRefreshRequest request,
        TokenRefreshHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }

    public static async Task<IResult> TokenRevokeRoute(
        TokenRevokeRequest request,
        TokenRevokeHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { error = result.Error });
    }
}