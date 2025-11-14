using ChatApp.Chat.Domain;
using ChatApp.Chat.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace ChatApp.Chat.Infrastructure.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(ChatParticipant chatParticipant, Message message);
    public Task ReceiveSystemMessage(Message message);
    public Task UserJoined(Message message, ChatParticipant chatParticipant);
    public Task UserLeft(Message message, ChatParticipant chatParticipant);
}


[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ChatDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;
    // redis
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

        // var isParticipant = await _dbContext.
    }

    public async Task SendMessage(Message message)
    {

    }
    

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
    

    private Guid GetUserId()
    {
        var claim = (Context.User?.FindFirst("sub")?.Value) ?? throw new HubException("Not authenticated");
        return Guid.Parse(claim);
    }
}