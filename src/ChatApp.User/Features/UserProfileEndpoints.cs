namespace ChatApp.User.Features;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/user")
            .WithTags("UserProfiles");

        

        return app;
    }

}