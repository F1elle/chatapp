using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Chat.SendMessage;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Chat.Infrastructure.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ChatApp.Chat.Infrastructure.Hubs;

// TODO: make this file less bullshit
// TODO: cancellation tokens
// TODO: exception handlers
// TODO: handle multiple devices connection

public interface IChatClient // TODO: move this interface somewhere else
{
    public Task ReceiveMessage(Message message);
    public Task ReceiveAdminMessage(Message message);
    public Task ReceiveSystemMessage(string message);
    public Task ReceiveNotification(Message message); // TODO: replace message with notification
    public Task UserJoined(Guid userId);
    public Task UserLeft(Guid userId);
}


[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ChatDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<ChatHub> _logger;
    private readonly SendMessageHandler _handler;

    public ChatHub(
        ChatDbContext dbContext,
        IConnectionMultiplexer redis, 
        ILogger<ChatHub> logger,
        SendMessageHandler sendMessageHandler)
    {
        _dbContext = dbContext;
        _redis = redis;
        _logger = logger;
        _handler = sendMessageHandler;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.UserGroup(userId));

        _logger.LogInformation("User {UserId} connected", userId);

        await base.OnConnectedAsync();
    }

    public async Task JoinChat(Guid chatId)
    {
        var userId = GetUserId();
        var redis = _redis.GetDatabase();

        var isParticipant = await _dbContext.ChatParticipants
            .AnyAsync(cp => cp.ChatId == chatId && cp.UserId == userId);

        if (!isParticipant)
            throw new HubException("Access denied");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(chatId));

        await redis.SetAddAsync(RedisKeys.ChatActive(chatId), userId.ToString());

        if (!Context.Items.ContainsKey("OpenChats"))
            Context.Items["OpenChats"] = new HashSet<Guid>();

        var OpenChats = Context.Items["OpenChats"] as HashSet<Guid>;
        OpenChats!.Add(chatId);

        _logger.LogInformation(
            "User {UserId} joined and marked active in chat {ChatId}",
            userId,
            chatId
        );
        
        await Clients.OthersInGroup(SignalRGroups.ChatGroup(chatId))
            .UserJoined(userId);
    }

    public async Task LeaveChat(Guid chatId)
    {
        var userId = GetUserId();
        var redis = _redis.GetDatabase();

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(chatId));
        await redis.SetRemoveAsync(RedisKeys.ChatActive(chatId), userId.ToString());

        if (Context.Items.TryGetValue("OpenChats", out var value)
            && value is HashSet<Guid> openChats)
        {
            openChats.Remove(chatId);
        }

        _logger.LogInformation(
            "User {UserId} left and marked inactive in chat {ChatId}",
            userId,
            chatId
        );

        await Clients.OthersInGroup(SignalRGroups.ChatGroup(chatId))
            .UserLeft(userId);
    }

    public async Task SendMessage(string messageContent, Guid chatId, CancellationToken ct) // make SendMessageRequest
    {
        var senderId = GetUserId();
        var redis = _redis.GetDatabase();

        

        var response = await _handler.Handle(new SendMessageRequest(senderId, chatId, messageContent), ct); 

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
        
        if (userId != null)
        {
            var redis = _redis.GetDatabase();

            if (Context.Items.TryGetValue("OpenChats", out var chatsObj))
            {
                var openChats = chatsObj as HashSet<Guid>;
                if (openChats != null)
                {
                    foreach (var chatId in openChats)
                    {
                        await redis.SetRemoveAsync(RedisKeys.ChatActive(chatId), userId.ToString());

                        await Clients.OthersInGroup(SignalRGroups.ChatGroup(chatId))
                            .UserLeft((Guid)userId);
                    }
                }
            }
        }

        _logger.LogInformation("User {UserId} disconnected", userId);

        await base.OnDisconnectedAsync(exception);
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
