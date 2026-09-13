using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.Common.Contracts;

public sealed record MessagePreview(
    string SenderName,
    string? MessageContent,
    MessageType Type,
    DateTime SentAt
);
