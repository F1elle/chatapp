using ChatApp.Chat.Contracts;
using ChatApp.Chat.Features.Chat.CloseChat;
using ChatApp.Chat.Features.Chat.OpenChat;
using ChatApp.Chat.Features.Chat.SendMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Chat.Infrastructure.Hubs;

// TODO: make this file less bullshit
// TODO: exception handlers
// TODO: handle multiple devices connection
// TODO: functional extensions
// TODO: dtos

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly SendMessageHandler _sendMessageHandler;
    private readonly OpenChatHandler _openChatHandler;
    private readonly CloseChatHandler _closeChatHandler;

    public ChatHub(
        ILogger<ChatHub> logger,
        SendMessageHandler sendMessageHandler,
        OpenChatHandler openChatHandler,
        CloseChatHandler closeChatHandler)
    {
        _logger = logger;
        _sendMessageHandler = sendMessageHandler;
        _openChatHandler = openChatHandler;
        _closeChatHandler = closeChatHandler;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.UserGroup(userId));

        _logger.LogInformation("User {UserId} connected", userId);

        await base.OnConnectedAsync();
    }

    public async Task OpenChat(Guid chatId, CancellationToken ct)
    {
        var userId = GetUserId();

        var response = await _openChatHandler.Handle(new OpenChatRequest(chatId, userId), ct);

        if (!response.IsSuccess)
            throw new HubException(response.Error);

        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(chatId));

        TrackOpenChat(chatId);

        _logger.LogInformation(
            "User {UserId} joined and marked active in chat {ChatId}",
            userId,
            chatId
        );
    }

    public async Task LeaveChat(Guid chatId, CancellationToken ct)
    {
        var userId = GetUserId();

        await _closeChatHandler.Handle(new CloseChatRequest(chatId, userId), ct);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(chatId));

        UntrackOpenChat(chatId);

        _logger.LogInformation(
            "User {UserId} left and marked inactive in chat {ChatId}",
            userId,
            chatId
        );
    }

    public async Task SendMessage(string messageContent, Guid chatId, CancellationToken ct) // make SendMessageRequest
    {
        var senderId = GetUserId();

        var response = await _sendMessageHandler.Handle(new SendMessageRequest(senderId, chatId, messageContent), ct); 

        if (!response.IsSuccess)
            throw new HubException(response.Error);

        var message = response.Value.Message;
        var inactiveParticipantIds = response.Value.InactiveParticipantIds;

        // sending message to active participants 
        await Clients.Group(SignalRGroups.ChatGroup(chatId))
            .ReceiveMessage(new MessageDto(
                message.Id,
                message.ChatId,
                message.ParticipantSenderId,
                message.Type,
                message.Content,
                message.SentAt
            ));

        _logger.LogInformation(
            "Message {MessageId} sent to active users in chat {ChatId}",
            message.Id,
            message.ChatId
        );

        foreach (var participant in inactiveParticipantIds)
        {
            await Clients.Group(SignalRGroups.UserGroup(participant))
                .ReceiveNotification(message);
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
            await _closeChatHandler.Handle(new CloseChatRequest(chatId, userId.Value), CancellationToken.None);
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
        if (Context.Items.TryGetValue("OpenChats", out var value)
            && value is HashSet<Guid> openChats)
        {
            openChats.Remove(chatId);
        }
    }

    private HashSet<Guid> GetTrackedChats()
    {
        return Context.Items.TryGetValue("OpenChats", out var value)
            && value is HashSet<Guid> trackedChats
            ? trackedChats
            : new HashSet<Guid>();
    }

    private Guid GetUserId()
    {
        var id = TryGetUserId();
        return id ?? throw new HubException("Unauthorized");
    }

    private Guid? TryGetUserId()
    {
        var claim = Context.User?.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var result) ? result : null;
    }
}
