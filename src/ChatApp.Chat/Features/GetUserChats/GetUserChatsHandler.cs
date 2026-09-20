using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Features.Common.Contracts;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common;
using ChatApp.Common.Abstractions;
using ChatApp.Common.Extensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.GetUserChats;

public class GetUserChatsHandler
    : IHandler<GetUserChatsQuery, Result<PagedResult<ChatListItem, ChatCursor>, ChatError>>
{
    private readonly ChatDbContext _dbContext;

    public GetUserChatsHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<ChatListItem, ChatCursor>, ChatError>> Handle(
        GetUserChatsQuery query,
        CancellationToken ct
    )
    {
        var dbQuery = _dbContext
            .Chats.Where(c => c.ChatParticipants.Any(cp => cp.UserId == query.UserId))
            .OrderByDescending(c => c.LastUpdateAt)
            .ThenByDescending(c => c.Id)
            .Include(c => c.LastMessage.Sender)
            .AsNoTracking();

        if (query.Cursor is { } cursor)
        {
            dbQuery = dbQuery.Where(c =>
                c.LastUpdateAt < cursor.LastUpdateAt
                || (c.LastUpdateAt == cursor.LastUpdateAt && c.Id.CompareTo(cursor.ChatId) < 0)
            );
        }

        return await dbQuery.ToPagedResult(
            query.PageSize,
            v => new ChatListItem(
                v.Id,
                v.Name,
                v.Type,
                v.CreatedAt,
                new MessagePreview(
                    v.LastMessage.Sender.Name,
                    v.LastMessage.Content,
                    v.LastMessage.Type,
                    v.LastMessage.SentAt
                )
            ),
            v => new ChatCursor(v.LastUpdateAt, v.Id),
            ct
        );
    }
}
