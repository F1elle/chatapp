using ChatApp.Chat.Api.Contracts;

namespace ChatApp.Chat.Api.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(MessageResponse message);
}
