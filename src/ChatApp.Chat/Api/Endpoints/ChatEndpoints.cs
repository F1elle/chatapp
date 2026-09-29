using System.Security.Claims;
using ChatApp.Chat.Api.Extensions;
using ChatApp.Chat.Common.Extensions;
using ChatApp.Chat.Features.CreateChat;
using ChatApp.Chat.Features.GetChatMessages;
using ChatApp.Chat.Features.GetUserChats;
using ChatApp.Chat.Features.SendMessage;

namespace ChatApp.Chat.Api.Endpoints;

public static class ChatEndpoints
{
    public static RouteGroupBuilder MapChatEndpoints(this RouteGroupBuilder group)
    {
        group
            .MapPost("/create", ChatCreateRoute)
            .WithName("CreateChat")
            .WithSummary("Create a new chat")
            .RequireAuthorization();

        group
            .MapPost("/message", SendMessageRoute)
            .WithName("SendMessage")
            .WithSummary("Send a message")
            .RequireAuthorization();

        // app.MapMethods("/search", ["QUERY"], (SearchFilter filter) =>
        // {
        //     return Results.Ok();
        // });

        // group
        //     .MapPost("/join", JoinChatRoute)
        //     .WithName("JoinChat")
        //     .WithSummary("Join an existing chat")
        //     .RequireAuthorization();

        group
            .MapGet("/list", GetUserChatsRoute)
            .WithName("GetUserChats")
            .WithSummary("Returns paged chats")
            .RequireAuthorization();

        group
            .MapGet("/{chatId}/messages", GetChatMessagesRoute)
            .WithName("GetChatMessages")
            .WithSummary("Returns paged messages from the chat")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> ChatCreateRoute(
        CreateChatCommand request,
        CreateChatHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.Handle(request, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    // private static async Task<IResult> JoinChatRoute(
    //     JoinChatRequest request,
    //     JoinChatHandler handler,
    //     CancellationToken ct
    // )
    // {
    //     var result = await handler.Handle(request, ct);
    //     if (!result.IsSuccess)
    //     {
    //         return Results.BadRequest(new { error = result.Error });
    //     }
    //     return Results.Ok(new { message = "Joined chat successfully." });
    // }

    private static async Task<IResult> GetUserChatsRoute(
        ChatListCursor? cursor,
        int pageSize,
        GetUserChatsHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var query = new GetUserChatsQuery((Guid)userId, cursor, pageSize);
        var result = await handler.Handle(query, ct);

        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetChatMessagesRoute(
        Guid chatId,
        Guid? cursor,
        int pageSize,
        GetChatMessagesHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var query = new GetChatMessagesQuery(chatId, userId.Value, cursor, pageSize);
        var result = await handler.Handle(query, ct);

        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    // TODO: implement that
    private static async Task<IResult> SendMessageRoute(
        Guid chatId,
        string content,
        SendMessageHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var query = new SendMessageCommand(chatId, userId.Value, content);
        var result = await handler.Handle(query, ct);

        // TODO: call hub

        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }
}
