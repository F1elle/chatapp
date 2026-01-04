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
    public SendMessageHandler(
        ChatDbContext dbContext,
        IChatPresenceService chatPresenceService,
        IChatAccessService chatAccessService)
    {
        _dbContext = dbContext;
        _presenceService = chatPresenceService;
        _accessService = chatAccessService;
    }
    
    public async Task<Result<SendMessageResponse>> Handle(SendMessageRequest request, CancellationToken ct) 
    {
        var hasAccess = await _accessService.CanAccessChatAsync(request.SenderId, request.ChatId, ct);       

        if (!hasAccess)
        {
            return Result.Failure<SendMessageResponse>("Access denied");
        }

        // defining inactive users
        var participants = await _dbContext.ChatParticipants
            .Where(cp => cp.ChatId == request.ChatId && cp.UserId != request.SenderId)
            .Select(cp => cp.UserId)
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
            ParticipantSenderId = request.SenderId,
            ChatId = request.ChatId,
            Content = request.Content,
            Type = request.Type
        };

        _dbContext.Add(message);
        await _dbContext.SaveChangesAsync(ct);

        return new SendMessageResponse(message, inactiveParticipantIds);
    }
}