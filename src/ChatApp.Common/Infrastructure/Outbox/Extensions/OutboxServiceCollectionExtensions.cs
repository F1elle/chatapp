using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;

namespace ChatApp.Common.Infrastructure.Outbox.Extensions;

public static class OutboxServiceCollectionOutboxExtensions
{
    public static IServiceCollection AddOutbox<TDbContext>(
        this IServiceCollection services,
        params Type[] eventTypes
    )
        where TDbContext : DbContext
    {
        services.AddScoped<IOutboxWriter, OutboxWriter<TDbContext>>();
        services.AddHostedService<OutboxPublisherBackgroundService<TDbContext>>();
        services.AddSingleton<IOutboxBus>(sp => new RebusOutboxBus(
            sp.GetRequiredService<IBus>(),
            eventTypes
        ));
        return services;
    }
}
