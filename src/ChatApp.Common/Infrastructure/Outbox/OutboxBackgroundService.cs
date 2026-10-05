using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatApp.Common.Infrastructure.Outbox;

public class OutboxPublisherBackgroundService<TDbContext> : BackgroundService
    where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOutboxBus _bus;
    private readonly ILogger<OutboxPublisherBackgroundService<TDbContext>> _logger;

    public OutboxPublisherBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOutboxBus bus,
        ILogger<OutboxPublisherBackgroundService<TDbContext>> logger
    )
    {
        _scopeFactory = scopeFactory;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TDbContext>();

            var pending = await db.Set<OutboxMessage>()
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(50)
                .ToListAsync(ct);

            foreach (var msg in pending)
            {
                try
                {
                    await _bus.Publish(msg.Type, msg.Payload, ct);
                    msg.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    msg.RetryCount++;
                    msg.LastError = ex.Message;
                    _logger.LogError(
                        ex,
                        "Failed to publish outbox message {MessageId} of type {Type}",
                        msg.Id,
                        msg.Type
                    );
                }
            }

            if (pending.Count > 0)
                await db.SaveChangesAsync(ct);

            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }
    }
}
