using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.Chat.CreateChat;

public sealed record CreateChatRequest(
    Guid CreatedBy,
    string? Name,
    List<Guid> ParticipantIds,
    ChatType Type
);