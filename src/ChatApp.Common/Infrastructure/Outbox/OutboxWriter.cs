using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Common.Infrastructure.Outbox;

public interface IOutboxWriter
{
    void Enqueue<TEvent>(TEvent domainEvent)
        where TEvent : class;
}

// TDbContext must contain Set<OutboxMessage>
public class OutboxWriter<TDbContext> : IOutboxWriter
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public OutboxWriter(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Enqueue<TEvent>(TEvent domainEvent)
        where TEvent : class
    {
        var message = new OutboxMessage
        {
            Type = typeof(TEvent).Name,
            Payload = JsonSerializer.Serialize(domainEvent),
        };
        _dbContext.Set<OutboxMessage>().Add(message);
    }
}
