using ChatApp.Chat.Domain.Enums;
using ChatApp.Chat.Features.GetChatMessages;
using ChatApp.Chat.Features.SendMessage;

namespace ChatApp.Chat.Api.Contracts;

public sealed record CreateChatRequest(string Name, List<Guid> ParticipantIds, ChatType ChatType);

public sealed record SendMessageRequest(Guid ChatId, string Content);

public sealed record GetUserChatsRequest(DateTime? LastUpdateAt, Guid? ChatId, int PageSize = 20);

public sealed record GetChatMessagesRequest(Guid ChatId, Guid? Cursor = null, int PageSize = 20);

public sealed record MessageResponse(
    Guid Id,
    Guid SenderId,
    string? Content,
    MessageType Type,
    Guid ChatId,
    DateTime SentAt,
    List<Guid> AttachmentIds
)
{
    public MessageResponse(SendMessageResult message)
        : this(
            message.Id,
            message.SenderId,
            message.Content,
            message.Type,
            message.ChatId,
            message.SentAt,
            message.AttachmentIds
        ) { }

    public MessageResponse(ChatMessageItem message)
        : this(
            message.Id,
            message.SenderId,
            message.Content,
            message.Type,
            message.ChatId,
            message.SentAt,
            message.AttachmentIds
        ) { }
};
