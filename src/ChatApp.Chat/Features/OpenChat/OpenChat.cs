namespace ChatApp.Chat.Features.OpenChat;


public sealed record OpenChatRequest(Guid ChatId, Guid UserId);
public sealed record OpenChatResponse(Guid ParticipantId); // TODO: finish it 