using ChatApp.Chat.Contracts;
using ChatApp.Chat.Domain;

namespace ChatApp.Chat.Infrastructure.Hubs;

public interface IChatClient // TODO: move this interface somewhere else later
{
    public Task ReceiveMessage(MessageDto message);
    public Task ReceiveAdminMessage(Message message);
    public Task ReceiveSystemMessage(string message);
    public Task ReceiveNotification(Message message);  
    public Task UserJoined(Guid userId); // TODO: that will be for real join chat operation from now on
    public Task UserLeft(Guid userId);
}