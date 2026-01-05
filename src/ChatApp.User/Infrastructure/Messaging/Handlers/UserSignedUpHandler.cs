using ChatApp.Common.Infrastructure.Messaging.Events;
using ChatApp.User.Features.CreateUserProfile;
using Rebus.Handlers;

namespace ChatApp.User.Infrastructure.Messaging.Handlers;

public class UserSignedUpHandler : IHandleMessages<UserSignedUpEvent>
{
    private readonly CreateUserProfileHandler _handler;
    private readonly ILogger<UserSignedUpHandler> _logger;

    public UserSignedUpHandler(
        CreateUserProfileHandler handler,
        ILogger<UserSignedUpHandler> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task Handle(UserSignedUpEvent message)
    {
        _logger.LogInformation("Received UserSignedUpEvent for user {UserId}", message.UserId);

        try
        {
            var request = new CreateUserProfileRequest(
                Id: message.UserId,
                Email: message.Email,
                DisplayName: message.DisplayName,
                CreatedAt: message.SignedUpAt);

            var result = await _handler.Handle(request, CancellationToken.None);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Profile created for user {UserId}",
                    message.UserId);
            }
            else
            {
                _logger.LogError(
                    "Failed to create profile for user {UserId}: {Error}",
                    message.UserId,
                    result.Error
                );
                throw new Exception(result.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UserSignedUpEvent for {UserId}",
            message.UserId);

            throw;
        }
    }
}