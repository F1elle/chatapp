using ChatApp.User.Features.UserProfile.GetUserProfile;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace ChatApp.User.Features;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/user")
            .WithTags("UserProfiles");

        group.MapGet("/{id}", GetUserProfileRoute)
            .WithName("GetUserProfile")
            .RequireAuthorization();

        return app;
    }
    
    private static async Task<IResult> GetUserProfileRoute(
        Guid id,
        GetUserProfileHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(id, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

}