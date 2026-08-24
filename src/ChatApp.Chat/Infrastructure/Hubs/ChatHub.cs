using ChatApp.Chat.Contracts;
using ChatApp.Chat.Features.ActivateChat;
using ChatApp.Chat.Features.SendMessage;
using ChatApp.Chat.Features.SuspendChat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Chat.Infrastructure.Hubs;

// TODO: exception handlers
// TODO: handle multiple devices connection
// TODO: functional extensions
// TODO: dtos

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly SendMessageHandler _sendMessageHandler;
    private readonly ActivateChatHandler _activateChatHandler;
    private readonly SuspendChatHandler _suspendChatHandler;

    public ChatHub(
        ILogger<ChatHub> logger,
        SendMessageHandler sendMessageHandler,
        ActivateChatHandler activateChatHandler,
        SuspendChatHandler suspendChatHandler
    )
    {
        _logger = logger;
        _sendMessageHandler = sendMessageHandler;
        _activateChatHandler = activateChatHandler;
        _suspendChatHandler = suspendChatHandler;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.UserGroup(userId));

        _logger.LogInformation("User {UserId} connected", userId);

        await base.OnConnectedAsync();
    }

    public async Task<Guid> OpenChat(Guid chatId)
    {
        var userId = GetUserId();

        var response = await _activateChatHandler.Handle(
            new ActivateChatRequest(chatId, userId),
            Context.ConnectionAborted
        );

        if (!response.IsSuccess)
            throw new HubException(response.Error);

        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(chatId));

        TrackOpenChat(chatId);

        _logger.LogInformation(
            "User {UserId} joined and marked active in chat {ChatId}",
            userId,
            chatId
        );

        return response.Value.ParticipantId;
    }

    public async Task LeaveChat(Guid chatId)
    {
        var userId = GetUserId();

        await _suspendChatHandler.Handle(
            new SuspendChatRequest(chatId, userId),
            Context.ConnectionAborted
        );

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(chatId));

        UntrackOpenChat(chatId);

        _logger.LogInformation(
            "User {UserId} left and marked inactive in chat {ChatId}",
            userId,
            chatId
        );
    }

    public async Task SendMessage(string messageContent, Guid chatId) // make SendMessageRequest
    {
        var senderId = GetUserId();

        _logger.LogInformation("User with Id: {Id} is sending a message", senderId);

        var response = await _sendMessageHandler.Handle(
            new SendMessageRequest(senderId, chatId, messageContent),
            Context.ConnectionAborted
        );

        if (!response.IsSuccess)
        {
            _logger.LogError("Error occured during sending a message: {Error}", response.Error);
            throw new HubException(response.Error);
        }

        var message = response.Value.Message;
        var inactiveParticipantIds = response.Value.InactiveParticipantIds;

        // sending message to active participants
        await Clients.Group(SignalRGroups.ChatGroup(chatId)).ReceiveMessage(message);

        _logger.LogInformation(
            "Message {MessageId} sent to active users in chat {ChatId}",
            message.Id,
            message.ChatId
        );

        foreach (var participant in inactiveParticipantIds)
        {
            await Clients.Group(SignalRGroups.UserGroup(participant)).ReceiveNotification(message);
        }

        // TODO: maybe make it async

        // var tasks = participants
        // .Where(p => !activeParticipants.Contains(p))
        // .Select(p => Clients.Group(SignalRGroups.UserGroup(p)).ReceiveNotification(message));
        // await Task.WhenAll(tasks);

        _logger.LogInformation(
            "Sent {Count} notifications for message {MessageId} in chat {ChatId}",
            inactiveParticipantIds.Count,
            message.Id,
            message.ChatId
        );
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = TryGetUserId();

        if (userId is null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        var openChats = GetTrackedChats();

        foreach (var chatId in openChats)
        {
            await _suspendChatHandler.Handle(
                new SuspendChatRequest(chatId, userId.Value),
                CancellationToken.None
            );
        }

        _logger.LogInformation("User {UserId} disconnected", userId);

        await base.OnDisconnectedAsync(exception);
    }

    private void TrackOpenChat(Guid chatId)
    {
        if (!Context.Items.ContainsKey("OpenChats"))
            Context.Items["OpenChats"] = new HashSet<Guid>();

        (Context.Items["OpenChats"] as HashSet<Guid>)!.Add(chatId);
    }

    private void UntrackOpenChat(Guid chatId)
    {
        if (
            Context.Items.TryGetValue("OpenChats", out var value)
            && value is HashSet<Guid> openChats
        )
        {
            openChats.Remove(chatId);
        }
    }

    private HashSet<Guid> GetTrackedChats()
    {
        return
            Context.Items.TryGetValue("OpenChats", out var value)
            && value is HashSet<Guid> trackedChats
            ? trackedChats
            : new HashSet<Guid>();
    }

    private Guid GetUserId()
    {
        var id = TryGetUserId();
        _logger.LogInformation("Veryfing user {Id} ID", id);
        return id ?? throw new HubException("Unauthorized");
    }

    private Guid? TryGetUserId()
    {
        var claim =
            Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var result) ? result : null;
    }
}
