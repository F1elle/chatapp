using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.GetUserChats;

public class GetUserChatsHandler
    : IHandler<GetUserChatsQuery, Result<GetUserChatsResult, ChatError>>
{
    private readonly ChatDbContext _dbContext;

    public GetUserChatsHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetUserChatsResult, ChatError>> Handle(
        GetUserChatsQuery query,
        CancellationToken ct
    )
    {
        var dbQuery = _dbContext
            .Chats.Where(c => c.ChatParticipants.Any(cp => cp.UserId == query.UserId))
            .AsNoTracking();

        if (query.Cursor.HasValue)
        {
            dbQuery = dbQuery.Where(c => (c.LastMessageAt ?? c.CreatedAt) < query.Cursor.Value);
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
