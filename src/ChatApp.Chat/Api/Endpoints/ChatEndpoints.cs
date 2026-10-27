using System.Security.Claims;
using ChatApp.Chat.Api.Contracts;
using ChatApp.Chat.Api.Extensions;
using ChatApp.Chat.Api.Hubs;
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
            .MapPost("/create", CreateChatRoute)
            .WithName("CreateChat")
            .WithSummary("Create a new chat")
            .RequireAuthorization();

        group
            .MapPost("/messages", SendMessageRoute)
            .WithName("SendMessage")
            .WithSummary("Send a message")
            .RequireAuthorization();

        group
            .MapGet("/list", GetUserChatsRoute)
            .WithName("GetUserChats")
            .WithSummary("Returns paged chats")
            .RequireAuthorization();

        group
            .MapGet("/messages", GetChatMessagesRoute)
            .WithName("GetChatMessages")
            .WithSummary("Returns paged messages from the chat")
            .RequireAuthorization();

        return group;
    }

    private static async Task<IResult> CreateChatRoute(
        CreateChatRequest request,
        CreateChatHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var command = new CreateChatCommand(
            (Guid)userId,
            request.Name,
            request.ParticipantIds,
            request.ChatType
        );

        var result = await handler.Handle(command, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetUserChatsRoute(
        [AsParameters] GetUserChatsRequest request,
        GetUserChatsHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        if ((request.LastUpdateAt is null) != (request.ChatId is null))
            return Results.BadRequest("LastUpdateAt and ChatId must be provided together."); // TODO: proper errors

        if (request.PageSize is < 1 or > 100)
            return Results.BadRequest("PageSize must be between 1 and 100.");

        ChatListCursor? cursor =
            request.LastUpdateAt is not null && request.ChatId is not null
                ? new ChatListCursor(request.LastUpdateAt.Value, request.ChatId.Value)
                : null;

        var query = new GetUserChatsQuery((Guid)userId, cursor, request.PageSize);

        var result = await handler.Handle(query, ct);

        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetChatMessagesRoute(
        GetChatMessagesRequest request,
        GetChatMessagesHandler handler,
        ClaimsPrincipal claims,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var query = new GetChatMessagesQuery(
            (Guid)userId,
            request.ChatId,
            request.Cursor,
            request.PageSize
        );

        var result = await handler.Handle(query, ct);

        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }

    private static async Task<IResult> SendMessageRoute(
        SendMessageRequest request,
        SendMessageHandler handler,
        ClaimsPrincipal claims,
        IChatNotifier chatNotifier,
        CancellationToken ct
    )
    {
        var userId = claims.GetUserId();

        if (userId == null)
            return Results.Unauthorized();

        var command = new SendMessageCommand((Guid)userId, request.ChatId, request.Content);

        var result = await handler.Handle(command, ct);

        if (result.IsSuccess)
            await chatNotifier.NotifyNewMessage(
                request.ChatId,
                new MessageResponse(result.Value),
                ct
            );

        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
    }
}
