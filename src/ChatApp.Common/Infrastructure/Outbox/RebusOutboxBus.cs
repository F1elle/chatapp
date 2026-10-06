using System.Text.Json;
using Rebus.Bus;

namespace ChatApp.Common.Infrastructure.Outbox;

public class RebusOutboxBus : IOutboxBus
{
    private readonly IBus _bus;
    private readonly IReadOnlyDictionary<string, Type> _eventTypes;

    public RebusOutboxBus(IBus bus, IEnumerable<Type> knownEventTypes)
    {
        _bus = bus;
        _eventTypes = knownEventTypes.ToDictionary(t => t.Name);
    }

    public async Task Publish(string eventType, string payload, CancellationToken ct)
    {
        if (!_eventTypes.TryGetValue(eventType, out var clrType))
            throw new InvalidOperationException($"Unknown outbox event type: {eventType}");

        var deserialized =
            JsonSerializer.Deserialize(payload, clrType)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize outbox payload for {eventType}"
            );

        await _bus.Publish(deserialized);
    }
}
