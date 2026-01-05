using System.Security.Claims;
using ChatApp.Chat.Common.Extensions;
using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Features.CreateChat;
using ChatApp.Chat.Features.GetUserChats;
using ChatApp.Chat.Features.JoinChat;
using ChatApp.Chat.Infrastructure.Hubs;
using Microsoft.AspNetCore.Mvc;


namespace ChatApp.Chat.Features.Chat;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/chat")
            .WithTags("Chat");

        group.MapHub<ChatHub>("/hub") 
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

        group.MapGet("/list", GetUserChatsRoute)
            .WithName("GetUserChats")
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

    private static async Task<IResult> GetUserChatsRoute(
        [FromQuery] DateTime? cursor,
        [FromQuery] int pageSize,
        GetUserChatsHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct)
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var request = new GetUserChatsRequest((Guid)userId, cursor, pageSize);
        var result = await handler.Handle(request, ct);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}
