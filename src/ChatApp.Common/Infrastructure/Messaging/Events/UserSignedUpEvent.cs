namespace ChatApp.Common.Infrastructure.Messaging.Events;

public record UserSignedUpEvent(
    Guid UserId,
    string Email,
    string DisplayName,
    DateTime SignedUpAt
);