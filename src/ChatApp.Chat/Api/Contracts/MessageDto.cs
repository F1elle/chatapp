using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Api.Contracts;

public sealed record DeprMessageDto( // TODO: extend it
    Guid Id,
    Guid ChatId,
    ChatParticipantDto Sender,
    MessageType Type,
    string? Content,
    DateTime SentAt
);
