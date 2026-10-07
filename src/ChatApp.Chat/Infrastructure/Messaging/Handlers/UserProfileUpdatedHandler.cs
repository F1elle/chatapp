using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Infrastructure.Messaging.Events;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;

namespace ChatApp.Chat.Infrastructure.Messaging.Handlers;

public class UserProfileUpdatedHandler : IHandleMessages<UserProfileUpdatedEvent>
{
    private readonly ILogger<UserProfileUpdatedHandler> _logger;
    private readonly ChatDbContext _dbContext;

    public UserProfileUpdatedHandler(
        ChatDbContext dbContext,
        ILogger<UserProfileUpdatedHandler> logger
    )
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(UserProfileUpdatedEvent message)
    {
        _logger.LogInformation("Received UserProfileUpdatedEvent for user {UserId}", message.Id);

        try
        {
            var ups = await _dbContext.UserProfileSnapshots.FirstAsync(
                ups => ups.UserId == message.Id,
                CancellationToken.None
            );

            ups.UpdateFrom(ups.DisplayName, ups.AvatarUrl);

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing UserProfileUpdatedEvent for {UserId}",
                message.Id
            );

            throw;
        }
    }
}
