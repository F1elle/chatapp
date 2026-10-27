using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.SendMessage;

public sealed record SendMessageCommand(
    Guid SenderId,
    Guid ChatId,
    string Content // make it nullable in the future, add media
// MessageType Type = MessageType.Text              // add later
);

public sealed record SendMessageResult(
    Guid Id,
    Guid SenderId,
    string? Content,
    MessageType Type,
    Guid ChatId,
    DateTime SentAt,
    List<Guid> AttachmentIds
);
