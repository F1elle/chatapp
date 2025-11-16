using ChatApp.Chat.Domain;
using ChatApp.Chat.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ChatApp.Chat.Infrastructure.Hubs;

// TODO: make this file less bullshit

public interface IChatClient // TODO: move this interface somewhere else
{
    public Task ReceiveMessage(Message message);
    public Task ReceiveSystemMessage(Message message);
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

    public ChatHub(
        ChatDbContext dbContext,
        IConnectionMultiplexer redis, 
        ILogger<ChatHub> logger)
    {
        _dbContext = dbContext;
        _redis = redis;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

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
        
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

        await redis.SetAddAsync($"chat:{chatId}:acitve", userId.ToString());

        if (!Context.Items.ContainsKey("OpenChats"))
            Context.Items["OpenChats"] = new HashSet<Guid>();

        var OpenChats = Context.Items["OpenChats"] as HashSet<Guid>;
        OpenChats!.Add(chatId);

        _logger.LogInformation(
            "User {UserId} joined and marked active in chat {ChatId}",
            userId,
            chatId
        );
        
        await Clients.OthersInGroup(chatId.ToString())
            .UserJoined(userId);
    }

    public async Task LeaveChat(Guid chatId)
    {
        var userId = GetUserId();
        var redis = _redis.GetDatabase();

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        await redis.SetRemoveAsync($"chat:{chatId}:acive", userId.ToString());

        var openChats = Context.Items["OpenChats"] as HashSet<Guid>;
        openChats?.Remove(chatId);

        _logger.LogInformation(
            "User {UserId} left and marked inactive in chat {ChatId}",
            userId,
            chatId
        );

        await Clients.OthersInGroup(chatId.ToString())
            .UserLeft(userId);
    }

    public async Task SendMessage(Message message)
    {
        var senderId = GetUserId();
        var redis = _redis.GetDatabase();

        if (string.IsNullOrWhiteSpace(message.Content) || message.Content.Length > 4096)
        {
            await Clients.Caller.ReceiveSystemMessage("Error: invalid message"); // TODO: maybe change method
            return;
        }

        var isParticipant = await _dbContext.ChatParticipants
            .AnyAsync(cp => cp.ChatId == message.ChatId && cp.UserId == message.ParticipantSenderId);

        if (!isParticipant)
        {
            await Clients.Caller.ReceiveSystemMessage("Error: access denied");
            return;
        }

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync(); // TODO: cancellationToken


        await Clients.Group(message.ChatId.ToString())
            .ReceiveMessage(message);

        _logger.LogInformation(
            "Message {MessageId} sent to active users in chat {ChatId}",
            message.Id,
            message.ChatId
        );

        var participants = await _dbContext.ChatParticipants
            .Where(cp => cp.ChatId == message.ChatId && cp.UserId != message.ParticipantSenderId)
            .Select(cp => cp.UserId)
            .ToListAsync(); // TODO: ct

        var notificationsSent = 0;

        foreach (var participantId in participants)
        {
            var isActive = await redis.SetContainsAsync(
                $"chat:{message.ChatId}:active",
                participantId.ToString()
            );

            if (!isActive)
            {
                await Clients.Group($"user:{participantId}")
                    .ReceiveNotification(message);

                notificationsSent++;
            }
        }

        _logger.LogInformation(
            "Sent {Count} notifications for message {MessageId} in chat {ChatId}",
            notificationsSent,
            message.Id,
            message.ChatId
        );
    }
    

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var redis = _redis.GetDatabase();

        if (Context.Items.TryGetValue("OpenChats", out var chatsObj))
        {
            var openChats = chatsObj as HashSet<Guid>;
            if (openChats != null)
            {
                foreach (var chatId in openChats)
                {
                    await redis.SetRemoveAsync($"chat:{chatId}:active", userId.ToString());

                    await Clients.OthersInGroup(chatId.ToString())
                        .UserLeft(userId);
                }
            }
        }

        _logger.LogInformation("User {UserId} disconnected", userId);

        await base.OnDisconnectedAsync(exception);
    }
    

    private Guid GetUserId()
    {
        var claim = (Context.User?.FindFirst("sub")?.Value) ?? throw new HubException("Not authenticated");
        return Guid.Parse(claim);
    }
}