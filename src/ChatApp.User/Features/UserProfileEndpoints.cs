using ChatApp.User.Features.UserProfile.GetUserProfile;

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
        GetUserProfileHandler handler)
    {
        var result = await handler.Handle(id);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

}