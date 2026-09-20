using ChatApp.Chat.Domain.Enums;
using ChatApp.Chat.Features.Common.Contracts;
using ChatApp.Common;

namespace ChatApp.Chat.Features.GetUserChats;

public sealed record GetUserChatsQuery(Guid UserId, ChatCursor? Cursor = null, int PageSize = 20);

public sealed record GetUserChatsResult : PagedResult<ChatListItem, ChatCursor>;

public sealed record ChatCursor(DateTime LastUpdateAt, Guid ChatId);

public sealed record ChatListItem(
    Guid Id,
    string Name,
    ChatType Type,
    DateTime CreatedAt,
    MessagePreview MessagePreview
);
