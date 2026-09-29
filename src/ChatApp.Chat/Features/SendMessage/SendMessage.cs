namespace ChatApp.Chat.Features.SendMessage;

public sealed record SendMessageCommand(
    Guid ChatId,
    Guid SenderId,
    string Content // make it nullable in the future, add media
// MessageType Type = MessageType.Text              // add later
);
