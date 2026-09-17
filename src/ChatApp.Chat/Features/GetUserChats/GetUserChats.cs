using ChatApp.Chat.Domain.Enums;
using ChatApp.Chat.Features.Common.Contracts;
using ChatApp.Common;

namespace ChatApp.Chat.Features.GetUserChats;

public sealed record GetUserChatsQuery(
    Guid UserId,
    Guid? CursorChatId = null,
    DateTime? CursorLastMessageAt = null,
    int PageSize = 20
);

public sealed record GetUserChatsResult(
    Guid Id,
    string Name,
    ChatType Type,
    DateTime CreatedAt,
    MessagePreview MessagePreview
);

// public sealed record ChatPreview(
//     Guid Id,
//     string Name,
//     ChatType Type,
//     DateTime CreatedAt,
//     MessagePreview MessagePreview
// );

// public sealed record GetUserChatsResponse(
//     List<ChatPreviewDto> ChatPreviews,
//     DateTime? NextCursor,
//     bool HasMore
// );
