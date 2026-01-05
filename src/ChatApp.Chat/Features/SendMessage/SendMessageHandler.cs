using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Domain;
using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.SendMessage;

public class SendMessageHandler : IHandler<SendMessageRequest, Result<SendMessageResponse>>
{
    private readonly ChatDbContext _dbContext;
    private readonly IChatPresenceService _presenceService;
    private readonly IChatAccessService _accessService;
    private readonly ILogger<SendMessageHandler> _logger;
    public SendMessageHandler(
        ChatDbContext dbContext,
        IChatPresenceService chatPresenceService,
        IChatAccessService chatAccessService,
        ILogger<SendMessageHandler> logger)
    {
        _dbContext = dbContext;
        _presenceService = chatPresenceService;
        _accessService = chatAccessService;
        _logger = logger;
    }
    
    public async Task<Result<SendMessageResponse>> Handle(SendMessageRequest request, CancellationToken ct) 
    {
        var participantId = await _accessService.GetParticipantIdAsync(request.SenderId, request.ChatId, ct);       

        if (participantId == null)
        {
            return Result.Failure<SendMessageResponse>("Access denied");
        }

        // defining inactive users
        var participants = await _dbContext.ChatParticipants
            .Where(cp => cp.ChatId == request.ChatId && cp.UserId != request.SenderId)
            .Select(cp => cp.UserId) // TODO: maybe userID
            .ToListAsync(ct); 

        var activeParticipants = await _presenceService.GetActiveParticipantsAsync(request.ChatId, ct);
        
        var inactiveParticipantIds = participants.Except(activeParticipants).ToList();

        var validationResult = new SendMessageValidator().Validate(request); // TODO: inject

        if (!validationResult)
        {
            return Result.Failure<SendMessageResponse>("Validation failed");
        }

        var message = new Message() 
        {
            ParticipantSenderId = (Guid)participantId,
            ChatId = request.ChatId,
            Content = request.Content,
            Type = request.Type
        };

        _logger.LogInformation("User with Id {Id} is trying to send a message {Message}", request.SenderId, request.Content);       

        _dbContext.Add(message);
        await _dbContext.SaveChangesAsync(ct);

        return new SendMessageResponse(message, inactiveParticipantIds);
    }
}