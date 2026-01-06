using ChatApp.Chat.Domain;
using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Contracts;

public sealed record MessageDto( // TODO: extend it 
    Guid Id,
    Guid ChatId,
    ChatParticipantDto Sender,
    MessageType Type,
    string? Content,
    DateTime SentAt
);