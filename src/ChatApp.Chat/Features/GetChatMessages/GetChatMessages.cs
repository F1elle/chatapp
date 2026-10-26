namespace ChatApp.Chat.Features.GetChatMessages;

public sealed record GetChatMessagesQuery(
    Guid UserId,
    Guid ChatId,
    Guid? Cursor = null,
    int PageSize = 20
);
