using ChatApp.Chat.Features.CreateChat;
using ChatApp.Chat.Features.JoinChat;
using ChatApp.Chat.Infrastructure.Hubs;

namespace ChatApp.Chat.Features.Chat;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/chat")
            .WithTags("Chat");

        group.MapHub<ChatHub>("/hub")
            .WithName("ChatHub")
            .WithSummary("SignalR Chat Hub")
            .RequireAuthorization();

        group.MapPost("/create", ChatCreateRoute)
            .WithName("CreateChat")
            .WithSummary("Create a new chat")
            .RequireAuthorization();

        group.MapPost("/join", JoinChatRoute)
            .WithName("JoinChat")
            .WithSummary("Join an existing chat")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> ChatCreateRoute(
            CreateChatRequest request,
            CreateChatHandler handler,
            CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { error = result.Error });
        }

        return Results.Ok(new { message = "Chat created successfully." });
    }

    private static async Task<IResult> JoinChatRoute(
            JoinChatRequest request,
            JoinChatHandler handler,
            CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { error = result.Error });
        }
        return Results.Ok(new { message = "Joined chat successfully." });
    }
}