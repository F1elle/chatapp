namespace ChatApp.Chat.Api.Hubs;

internal static class SignalRGroups
{
    internal static string ChatGroup(Guid chatId) => chatId.ToString();

    // internal static string UserGroup(Guid userId) => $"user:{userId}";
}
