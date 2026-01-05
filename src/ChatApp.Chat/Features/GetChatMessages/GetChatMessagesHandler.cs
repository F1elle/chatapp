using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.GetChatMessages;


public class GetChatMessagesHandler : IHandler<GetChatMessagesRequest, Result<GetChatMessagesResponse>>
{
    private readonly ILogger<GetChatMessagesHandler> _logger;
    private readonly ChatDbContext _dbContext;
    private readonly IChatAccessService _chatAccessService;

    public GetChatMessagesHandler(
        ILogger<GetChatMessagesHandler> logger,
        ChatDbContext dbContext,
        IChatAccessService chatAccessService)
    {
        _logger = logger;
        _dbContext = dbContext; 
        _chatAccessService = chatAccessService;
    }

    public async Task<Result<GetChatMessagesResponse>> Handle(GetChatMessagesRequest request, CancellationToken ct)
    {
        _logger.LogInformation("User with Id {Id} is trying to get messages from {ChatId}", request.UserId, request.ChatId);
        var participantId = await _chatAccessService.GetParticipantIdAsync(request.UserId, request.ChatId, ct);

        if (participantId == null)
        {
            return Result.Failure<GetChatMessagesResponse>("Not a chat member");
        }

        var query = _dbContext.Messages
            .Where(m => m.ChatId == request.ChatId);

        if (request.Cursor.HasValue)
        {
            query = query.Where(m => m.SentAt < request.Cursor.Value);
        }

        var orderedQuery = query.OrderByDescending(m => m.SentAt);

        var messages = await orderedQuery
            .Take(request.PageSize + 1)
            .ToListAsync();

        var hasMore = messages.Count > request.PageSize;

        if (hasMore)
        {
            messages = messages.Take(request.PageSize).ToList();
        }


        messages.Reverse();

        var nextCursor = messages.FirstOrDefault()?.SentAt;

        return new GetChatMessagesResponse(messages, nextCursor, hasMore);
    }
        
}