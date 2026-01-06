using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Contracts;
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
        var validationResult = new SendMessageValidator().Validate(request); // TODO: inject

        if (!validationResult)
        {
            return Result.Failure<SendMessageResponse>("Validation failed");
        }

        var participantId = await _accessService.GetParticipantIdAsync(request.SenderId, request.ChatId, ct);       

        if (participantId == null)
        {
            return Result.Failure<SendMessageResponse>("Access denied");
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        try
        {
            return await strategy.ExecuteAsync(async () =>
            {

                using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

                
                var message = Message.CreateTextMessage((Guid)participantId, request.ChatId, request.Content);

                _logger.LogInformation("User with Id {Id} is trying to send a message {Message}", request.SenderId, request.Content);       

                _dbContext.Add(message);

                await _dbContext.SaveChangesAsync(ct);

                await _dbContext.Chats
                    .Where(c => c.Id == request.ChatId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(c => c.LastMessageId, message.Id)
                        .SetProperty(c => c.LastMessageAt, message.SentAt), 
                    ct);

                await _dbContext.ChatParticipants
                            .Where(cp => cp.ChatId == request.ChatId && cp.UserId == request.SenderId)
                            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastReadMessageId, message.Id), ct);

                await transaction.CommitAsync(ct);


                // defining inactive users
                var participants = await _dbContext.ChatParticipants
                    .Where(cp => cp.ChatId == request.ChatId && cp.UserId != request.SenderId)
                    .Select(cp => cp.UserId) // TODO: maybe userID
                    .ToListAsync(ct); 

                var activeParticipants = await _presenceService.GetActiveParticipantsAsync(request.ChatId, ct);
                
                var inactiveParticipantIds = participants.Except(activeParticipants).ToList();


                return new SendMessageResponse(new MessageDto(
                    message.Id,
                    message.ChatId,
                    new ChatParticipantDto((Guid)participantId, request.SenderId),
                    message.Type,
                    message.Content,
                    message.SentAt), inactiveParticipantIds);
                
            });

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to send message and update counters");
            return Result.Failure<SendMessageResponse>("Database error");
        }

    }

}