namespace ChatApp.Chat.Features.Chat.OpenChat;


public sealed record OpenChatRequest(Guid ChatId, Guid UserId);
//public sealed record OpenChatResponse(bool HasAccess); // TODO: finish it 