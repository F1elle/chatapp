using ChatApp.Chat.Domain.Enums;
using ChatApp.Chat.Features.GetUserChats;

namespace ChatApp.Chat.Api.Contracts;

public sealed record CreateChatRequest(string Name, List<Guid> ParticipantIds, ChatType ChatType);

public sealed record SendMessageRequest(Guid ChatId, string Content);

public sealed record GetUserChatsRequest(DateTime? LastUpdateAt, Guid? ChatId, int PageSize = 20);

public sealed record GetChatMessagesRequest(Guid ChatId, Guid? Cursor = null, int PageSize = 20);
