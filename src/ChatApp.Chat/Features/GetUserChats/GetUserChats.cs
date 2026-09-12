namespace ChatApp.Chat.Features.GetUserChats;

public sealed record GetUserChatsQuery(
    Guid UserId,
    Guid? ChatId = null,
    DateTime? LastMessageAt = null,
    int PageSize = 20
);

public sealed record GetUserChatsResult();

// public sealed record GetUserChatsResponse(
//     List<ChatPreviewDto> ChatPreviews,
//     DateTime? NextCursor,
//     bool HasMore
// );
