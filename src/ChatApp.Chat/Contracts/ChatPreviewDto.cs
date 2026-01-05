using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Contracts;

public sealed record ChatPreviewDto( 
    Guid Id,
    string? Name,
    ChatType Type,
    DateTime? LastMessageAt,
    string? LastMessageContent,
    int UnreadCount,
    string? LastMessageSenderName
);