using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.Common.Contracts;

// Used when message preview is needed - notification, chat entry, etc.
// TODO: make this specific to chat list, use different for notifications
public sealed record MessagePreview(
    string SenderName,
    Guid ChatId,
    string? Content,
    MessageType Type,
    DateTime SentAt
);
