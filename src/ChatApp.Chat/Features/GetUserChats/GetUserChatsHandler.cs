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
            .OrderByDescending(c => c.LastMessageAt)
            .ThenByDescending(c => c.Id)
            .AsNoTracking();

        if (query.CursorChatId is { } cursorId && query.CursorLastMessageAt is { } cursorTime)
        {
            dbQuery = dbQuery.Where(c =>
                (c.LastMessageAt ?? c.CreatedAt) > cursorTime
                || (c.LastMessageAt == cursorTime && c.Id.CompareTo(cursorId) < 0)
            );
        }

        var chats = await dbQuery.Take(query.PageSize + 1).ToListAsync(ct);

        var hasMore = chats.Count > query.PageSize;
        // TODO: generic paged result; inherit UserChatsResponse from it

        if (hasMore)
        {
            chats = chats.Take(request.PageSize).ToList();
        }

        var nextCursor = chats.LastOrDefault()?.LastMessageAt;

        return new GetUserChatsResponse(chats, nextCursor, hasMore);
    }
}
