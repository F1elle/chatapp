namespace ChatApp.Common.Infrastructure.Messaging.Events;

public record UserProfileUpdatedEvent(Guid Id, string DisplayName, string? AvatarUrl);
