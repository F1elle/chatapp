using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Features.Common.Contracts;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common;
using ChatApp.Common.Abstractions;
using ChatApp.Common.Extensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.GetChatMessages;

public class GetChatMessagesHandler
    : IHandler<GetChatMessagesQuery, Result<PagedResult<MessageDto, Guid>, ChatError>>
{
    private readonly ChatDbContext _dbContext;
    private readonly IChatAccessService _chatAccessService;

    public GetChatMessagesHandler(ChatDbContext dbContext, IChatAccessService chatAccessService)
    {
        _dbContext = dbContext;
        _chatAccessService = chatAccessService;
    }

    public async Task<Result<PagedResult<MessageDto, Guid>, ChatError>> Handle(
        GetChatMessagesQuery query,
        CancellationToken ct
    )
    {
        var participantId = await _chatAccessService.GetParticipantIdAsync(
            query.UserId,
            query.ChatId,
            ct
        );

        if (participantId == null)
        {
            return ChatError.NotChatParticipant;
        }

        var dbQuery = _dbContext
            .Messages.Where(m => m.ChatId == query.ChatId)
            .OrderByDescending(m => m.Id)
            .AsNoTracking();

        if (query.Cursor is { } cursor)
        {
            dbQuery = dbQuery.Where(m => m.Id < cursor);
        }

        return await dbQuery.ToPagedResult(
            query.PageSize,
            m => new MessageDto(
                m.Id,
                m.SenderId,
                m.Content,
                m.Type,
                m.ChatId,
                m.SentAt,
                m.AttachmentIds
            ),
            m => m.Id,
            ct
        );
    }
}
