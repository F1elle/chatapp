using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Common.Infrastructure.Outbox.Extensions;

public static class OutboxServiceCollectionOutboxExtensions
{
    public static IServiceCollection AddOutbox<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IOutboxWriter, OutboxWriter<TDbContext>>();
        services.AddHostedService<OutboxPublisherBackgroundService<TDbContext>>();
        return services;
    }
}
