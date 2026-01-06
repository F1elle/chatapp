namespace ChatApp.Chat.Contracts;

public sealed record ChatParticipantDto( 
    Guid Id,
    Guid UserId
);