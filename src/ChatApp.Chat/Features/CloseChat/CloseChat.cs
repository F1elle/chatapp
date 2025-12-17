namespace ChatApp.Chat.Features.CloseChat;

public sealed record CloseChatRequest(Guid ChatId, Guid UserId);

public sealed record CloseChatResponse();