namespace ChatApp.Chat.Features.OpenChat;


public sealed record OpenChatRequest(Guid ChatId, Guid UserId);
public sealed record OpenChatResponse(); // TODO: finish it 