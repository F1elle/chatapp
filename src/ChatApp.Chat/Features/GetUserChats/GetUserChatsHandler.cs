using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using ChatApp.Common.Extensions;
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

        return await dbQuery.ToPagedResult<Domain.Chat, ChatListItem, string, GetUserChatsResult>(
            pageSize: 20,
            projector: (v) =>
                new ChatListItem(
                    v.Id,
                    v.Name,
                    v.Type,
                    v.CreatedAt,
                    new Common.Contracts.MessagePreview(
                        v.LastMessage.Sender.Name,
                        v.LastMessage.Content,
                        v.LastMessage.Type,
                        v.LastMessage.SentAt
                    )
                ),
            cursorSelector: (v) => $"{v.MessagePreview.SentAt}_{v.Id}",
            ct
        );
    }
}
