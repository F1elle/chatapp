using ChatApp.Chat.Domain;

namespace ChatApp.Chat.Features.Chat.SendMessage;

public sealed record SendMessageRequest(Message Message, ChatParticipant ChatParticipant);