using ChatApp.Chat.Domain;

namespace ChatApp.Chat.Features.Chat.SendMessage;

public sealed record SendMessageCommand (Message Message, ChatParticipant ChatParticipant);