using ChatApp.Chat.Domain;
using ChatApp.Chat.Domain.Enums;

namespace ChatApp.Chat.Features.SendMessage;

public sealed record SendMessageRequest(
    Guid SenderId,
    Guid ChatId,
    string Content, // make it nullable in the future
    MessageType Type = MessageType.Text);

public sealed record SendMessageResponse(
    Message Message,
    IReadOnlyCollection<Guid> InactiveParticipantIds
);
