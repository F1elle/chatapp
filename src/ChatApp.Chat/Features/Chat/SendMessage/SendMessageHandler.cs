using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Chat.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.Chat.SendMessage;

public class SendMessageHandler
{
    private readonly ChatDbContext _dbContext;
    private readonly IChatPresenceService _presenceService;
    public SendMessageHandler(
        ChatDbContext dbContext,
        IChatPresenceService chatPresenceService)
    {
        _dbContext = dbContext;
        _presenceService = chatPresenceService;
    }
    
    public async Task<SendMessageResponse> Handle(SendMessageRequest request, CancellationToken ct) // TODO: Result<Message>
    {
        var isParticipant = await _dbContext.ChatParticipants
            .AnyAsync(cp => cp.ChatId == request.ChatId && cp.UserId == request.SenderId);

        if (!isParticipant)
        {
            throw new Exception("Access denied");
        }


        // defining inactive users
        var participants = await _dbContext.ChatParticipants
            .Where(cp => cp.ChatId == request.ChatId && cp.UserId != request.SenderId)
            .Select(cp => cp.UserId)
            .ToListAsync(ct); 

        var activeParticipants = await _presenceService.GetActiveParticipantsAsync(request.ChatId, ct);
        
        var inactiveParticipantIds = new List<Guid>();
        foreach(var participant in participants)
        {
            if (!activeParticipants.Contains(participant))
            {
                inactiveParticipantIds.Add(participant);
            }
        }

        var message = new Message() // TODO: validation
        {
            ParticipantSenderId = request.SenderId,
            ChatId = request.ChatId,
            Content = request.Content,
            Type = request.Type
        };
        _dbContext.Add(message);
        await _dbContext.SaveChangesAsync(ct);

        return new (message, inactiveParticipantIds);
    }
}