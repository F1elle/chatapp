using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Contracts;

public sealed record MessageDto( // TODO: extend it 
    Guid Id,
    Guid ChatId,
    Guid SenderId,
    MessageType Type,
    string? Content,
    DateTime SentAt
);