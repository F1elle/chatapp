namespace ChatApp.Chat.Infrastructure.Redis;

internal static class RedisKeys
{
    internal static string ChatActive(Guid chatId) => $"chat:{chatId}:active";
}