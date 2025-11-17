using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Chat.CloseChat;
using ChatApp.Chat.Features.Chat.OpenChat;
using ChatApp.Chat.Features.Chat.SendMessage;
using ChatApp.Chat.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Chat.Infrastructure.Hubs;

// TODO: make this file less bullshit
// TODO: cancellation tokens
// TODO: exception handlers
// TODO: handle multiple devices connection
// TODO: functional extensions

public interface IChatClient // TODO: move this interface somewhere else
{
    public Task ReceiveMessage(Message message);
    public Task ReceiveAdminMessage(Message message);
    public Task ReceiveSystemMessage(string message);
    public Task ReceiveNotification(Message message); // TODO: replace message with notification
    public Task UserJoined(Guid userId); // TODO: that will be for real join chat operation now on
    public Task UserLeft(Guid userId);
}


[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ChatDbContext _dbContext;
    private readonly ILogger<ChatHub> _logger;
    private readonly SendMessageHandler _sendMessageHandler;
    private readonly OpenChatHandler _openChatHandler;
    private readonly CloseChatHandler _closeChatHandler;

    public ChatHub(
        ChatDbContext dbContext,
        ILogger<ChatHub> logger,
        SendMessageHandler sendMessageHandler,
        OpenChatHandler openChatHandler,
        CloseChatHandler closeChatHandler)
    {
        _dbContext = dbContext;
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

        if (!response.HasAccess)
            throw new HubException("Access denied");

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

        // sending message to active participants 
        await Clients.Group(SignalRGroups.ChatGroup(chatId))
            .ReceiveMessage(response.Message);

        _logger.LogInformation(
            "Message {MessageId} sent to active users in chat {ChatId}",
            response.Message.Id,
            response.Message.ChatId
        );

        foreach (var participant in response.InactiveParticipantIds)
        {
            await Clients.Group(SignalRGroups.UserGroup(participant))
                .ReceiveNotification(response.Message);
        }
       
        // TODO: maybe make it async 

        // var tasks = participants
        // .Where(p => !activeParticipants.Contains(p))
        // .Select(p => Clients.Group(SignalRGroups.UserGroup(p)).ReceiveNotification(message));
        // await Task.WhenAll(tasks);


        _logger.LogInformation(
            "Sent {Count} notifications for message {MessageId} in chat {ChatId}",
            response.InactiveParticipantIds.Count,
            response.Message.Id,
            response.Message.ChatId
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
