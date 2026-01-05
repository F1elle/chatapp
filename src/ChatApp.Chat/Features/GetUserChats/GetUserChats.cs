using ChatApp.Chat.Contracts;

namespace ChatApp.Chat.Features.GetUserChats;

public sealed record GetUserChatsRequest(
    Guid UserId,
    DateTime? Cursor = null, 
    int PageSize = 20);

public sealed record GetUserChatsResponse(
    List<ChatPreviewDto> ChatPreviews, 
    DateTime? NextCursor,
    bool HasMore);