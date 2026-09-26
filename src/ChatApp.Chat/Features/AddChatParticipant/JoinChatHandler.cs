using ChatApp.Chat.Domain;
using ChatApp.Chat.Domain.Enums;
using ChatApp.Chat.Features.Common;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Common.Abstractions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Features.AddChatParticipant;

public class JoinChatHandler : IHandler<AddChatParticipantCommand, Result<Guid, ChatError>>
{
    private readonly ChatDbContext _dbContext;

    public JoinChatHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid, ChatError>> Handle(
        AddChatParticipantCommand command,
        CancellationToken ct
    )
    {
        var chatType = await _dbContext
            .Chats.Where(c => c.Id == command.ChatId)
            .Select(c => (ChatType?)c.Type)
            .FirstOrDefaultAsync(ct);

        if (chatType is null)
            return ChatError.ChatNotFound;

        if (chatType != ChatType.Group)
            return ChatError.NotGroupChat;

        ChatParticipant chatParticipant = new(command.UserId, command.ChatId);

        _dbContext.Add(chatParticipant);
        await _dbContext.SaveChangesAsync(ct);

        return chatParticipant.Id;
    }
}
