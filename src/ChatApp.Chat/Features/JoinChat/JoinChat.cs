namespace ChatApp.Chat.Features.JoinChat;

public sealed record JoinChatRequest(Guid ChatId, Guid UserId);

public sealed record JoinChatResponse();