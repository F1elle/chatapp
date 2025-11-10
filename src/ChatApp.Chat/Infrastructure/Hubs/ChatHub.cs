using ChatApp.Chat.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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
    public async Task SendMessage(Message message) { }

    public override async Task OnConnectedAsync()
    {

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

}