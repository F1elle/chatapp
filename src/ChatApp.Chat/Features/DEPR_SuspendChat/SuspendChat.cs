namespace ChatApp.Chat.Features.SuspendChat;

public sealed record SuspendChatRequest(Guid ChatId, Guid UserId);

public sealed record SuspendChatResponse();
