namespace ChatApp.Chat.Features.Chat.JoinChat;

public sealed record JoinChatRequest(Guid ChatId, Guid UserId);

//public sealed record JoinChatResponse();