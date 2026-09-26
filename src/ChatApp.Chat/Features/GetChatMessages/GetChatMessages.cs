namespace ChatApp.Chat.Features.GetChatMessages;

public sealed record GetChatMessagesQuery(
    Guid ChatId,
    Guid UserId,
    Guid? Cursor = null,
    int PageSize = 20
);
