using ChatApp.Chat.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Api.Hubs;

// TODO: exception handlers
// TODO: handle multiple devices connection
// TODO: functional extensions
// TODO: dtos

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly ChatDbContext _dbContext;

    public ChatHub(ILogger<ChatHub> logger, ChatDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public override async Task OnConnectedAsync()
    {
        CancellationToken ct = Context.ConnectionAborted;

        var userId = GetUserId();

        var chatIds = await _dbContext
            .ChatParticipants.Where(cp => cp.UserId == userId)
            .Select(cp => cp.ChatId)
            .ToListAsync(ct);

        var tasks = chatIds.Select(id =>
            Groups.AddToGroupAsync(Context.ConnectionId, SignalRGroups.ChatGroup(id), ct)
        );

        await Task.WhenAll(tasks);

        _logger.LogInformation("User {UserId} connected", userId);

        // TODO: markConnected

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = TryGetUserId();

        if (userId is null)
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        _logger.LogInformation("User {UserId} disconnected", userId);

        await base.OnDisconnectedAsync(exception);
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
