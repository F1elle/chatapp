using ChatApp.Chat.Common.Abstractions;
using ChatApp.Chat.Contracts;
using ChatApp.Chat.Infrastructure.Data;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.GetUserChats;

public class GetUserChatsHandler : IHandler<GetUserChatsRequest, Result<GetUserChatsResponse>>
{
    private readonly ChatDbContext _dbContext;

    public GetUserChatsHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetUserChatsResponse>> Handle(
        GetUserChatsRequest request,
        CancellationToken ct)
    {
        var query = _dbContext.Chats
            .Where(c => c.ChatParticipants.Any(cp => cp.UserId == request.UserId))
            .AsNoTracking();

        if (request.Cursor.HasValue)
        {
            query = query.Where(c => (c.LastMessageAt ?? c.CreatedAt) < request.Cursor.Value);
        }

        var orderedQuery = query.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt);

        var chats = await orderedQuery
            .Take(request.PageSize + 1)
            .Select(c => new ChatPreviewDto(
                c.Id,
                c.Name,
                c.Type,
                c.CreatedAt,
                c.LastMessageAt,
                c.LastMessage != null 
                    ? c.LastMessage.Content ?? "No messages yet" 
                    : "No messages yet",
                c.LastMessage != null 
                    ? new ChatParticipantDto(
                        c.LastMessage.ParticipantSenderId,
                        c.LastMessage.ParticipantSender.UserId
                    )
                    : null
            ))
            .ToListAsync(ct);

        var hasMore = chats.Count > request.PageSize;

        if (hasMore)
        {
            chats = chats.Take(request.PageSize).ToList();
        }

        var nextCursor = chats.LastOrDefault()?.LastMessageAt;

        return new GetUserChatsResponse(chats, nextCursor, hasMore);
    }
}
