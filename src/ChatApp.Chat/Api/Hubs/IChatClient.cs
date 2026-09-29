using ChatApp.Chat.Features.Common.Contracts;

namespace ChatApp.Chat.Api.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(MessageDto message);
    public Task ReceiveSystemMessage(string message);
}
