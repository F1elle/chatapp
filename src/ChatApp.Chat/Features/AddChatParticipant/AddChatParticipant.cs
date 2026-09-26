namespace ChatApp.Chat.Features.AddChatParticipant;

public sealed record AddChatParticipantCommand(Guid ChatId, Guid UserId);
