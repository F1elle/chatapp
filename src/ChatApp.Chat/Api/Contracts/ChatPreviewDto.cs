using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Api.Contracts;

public sealed record ChatPreviewDto(
    Guid Id,
    string? Name,
    ChatType Type,
    DateTime CreatedAt,
    DateTime? LastMessageAt,
    string? LastMessageContent,
    ChatParticipantDto? LastMessageSender
);
