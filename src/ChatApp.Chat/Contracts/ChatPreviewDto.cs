using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Contracts;

public sealed record ChatPreviewDto( 
    Guid Id,
    string? Name,
    ChatType Type,
    DateTime CreatedAt,
    DateTime? LastMessageAt,
    string? LastMessageContent,
    ChatParticipantDto? LastMessageSender
);