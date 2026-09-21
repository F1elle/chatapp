using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common;
using ChatApp.Common.Abstractions;
using ChatApp.Common.Extensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.GetUserChats;

public class GetUserChatsHandler
    : IHandler<GetUserChatsQuery, Result<PagedResult<ChatListItem, ChatListCursor>, ChatError>>
{
    private readonly ChatDbContext _dbContext;

    public GetUserChatsHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<ChatListItem, ChatListCursor>, ChatError>> Handle(
        GetUserChatsQuery query,
        CancellationToken ct
    )
    {
        var dbQuery = _dbContext.Chats.Where(c =>
            c.Participants.Any(cp => cp.UserId == query.UserId)
        );

        if (query.Cursor is { } cursor)
        {
            dbQuery = dbQuery.Where(c =>
                c.LastUpdateAt < cursor.LastUpdateAt
                || (c.LastUpdateAt == cursor.LastUpdateAt && c.Id.CompareTo(cursor.ChatId) < 0)
            );
        }

        var projectedQuery = dbQuery
            .OrderByDescending(c => c.LastUpdateAt)
            .ThenByDescending(c => c.Id)
            .Select(c => new
            {
                Chat = c,
                SenderName = c.LastMessage == null
                    ? null
                    : _dbContext
                        .UserProfileSnapshots.Where(u => u.UserId == c.LastMessage.SenderId)
                        .Select(u => u.DisplayName)
                        .FirstOrDefault(),
            })
            .AsNoTracking();

        return await projectedQuery.ToPagedResult(
            query.PageSize,
            v => new ChatListItem(
                v.Chat.Id,
                v.Chat.Name ?? "Unknown",
                v.Chat.Type,
                v.Chat.CreatedAt,
                v.Chat.LastMessage != null
                    ? new MessagePreview(
                        v.SenderName ?? "Unknown",
                        v.Chat.LastMessage.Content,
                        v.Chat.LastMessage.Type,
                        v.Chat.LastMessage.SentAt
                    )
                    : null
            ),
            v => new ChatListCursor(v.Chat.LastUpdateAt, v.Chat.Id),
            ct
        );
    }
}
