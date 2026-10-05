namespace ChatApp.Common.Infrastructure.Outbox;

public interface IOutboxBus
{
    Task Publish(string eventType, string payload, CancellationToken ct);
}
