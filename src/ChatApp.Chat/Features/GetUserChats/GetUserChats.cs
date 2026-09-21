using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.GetUserChats;

public sealed record GetUserChatsQuery(
    Guid UserId,
    ChatListCursor? Cursor = null,
    int PageSize = 20
);

public sealed record ChatListCursor(DateTime LastUpdateAt, Guid ChatId);

public sealed record MessagePreview(
    string SenderName,
    string? Content,
    MessageType Type,
    DateTime SentAt
);

public sealed record ChatListItem(
    Guid Id,
    string Name,
    ChatType Type,
    DateTime CreatedAt,
    MessagePreview? MessagePreview
);
