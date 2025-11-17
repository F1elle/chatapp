namespace ChatApp.Chat.Features.Chat.CloseChat;

public sealed record CloseChatRequest(Guid ChatId, Guid UserId);

public sealed record CloseChatResponse();