using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.GetChatMessages;

public sealed record GetChatMessagesQuery(
    Guid UserId,
    Guid ChatId,
    Guid? Cursor = null,
    int PageSize = 20
);

public sealed record ChatMessageItem(
    Guid Id,
    Guid SenderId,
    string? Content,
    MessageType Type,
    Guid ChatId,
    DateTime SentAt,
    List<Guid> AttachmentIds
);
