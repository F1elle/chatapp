using ChatApp.Chat.Contracts;

namespace ChatApp.Chat.Infrastructure.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(MessageDto message);
    public Task ReceiveSystemMessage(string message);
}
