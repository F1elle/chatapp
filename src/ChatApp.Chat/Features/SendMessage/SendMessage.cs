namespace ChatApp.Chat.Features.SendMessage;

public sealed record SendMessageCommand(
    Guid SenderId,
    Guid ChatId,
    string Content // make it nullable in the future, add media
// MessageType Type = MessageType.Text              // add later
);
