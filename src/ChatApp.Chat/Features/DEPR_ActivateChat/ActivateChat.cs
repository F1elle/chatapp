namespace ChatApp.Chat.Features.ActivateChat;

public sealed record ActivateChatRequest(Guid ChatId, Guid UserId);

public sealed record ActivateChatResponse(Guid ParticipantId); // TODO: finish it 
