using ChatApp.Chat.Domain;
using ChatApp.Chat.Infrastructure.Data;

namespace ChatApp.Chat.Features.Chat.SendMessage;

public class SendMessageHandler
{
    private readonly ChatDbContext _dbContext;
    public SendMessageHandler(
        ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Handle(SendMessageCommand command)
    {
        _dbContext.Add(command.Message);
        await _dbContext.SaveChangesAsync();
    }
}