using ChatApp.Chat.Contracts;

namespace ChatApp.Chat.Features.GetChatMessages;

public sealed record GetChatMessagesRequest(
    Guid ChatId,
    Guid UserId,
    DateTime? Cursor = null,
    int PageSize = 20
);

public sealed record GetChatMessagesResponse(
    List<MessageDto> Messages,
    DateTime? NextCursor,
    bool HasMore
);