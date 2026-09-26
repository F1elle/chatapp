using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.Common.Contracts;

// Message DTO shared across handlers
public sealed record MessageDto(
    Guid Id,
    Guid SenderId,
    string? Content,
    MessageType Type,
    Guid ChatId,
    DateTime SentAt,
    List<Guid> AttachmentIds
);
