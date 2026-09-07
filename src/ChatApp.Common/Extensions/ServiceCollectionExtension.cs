using System.Reflection;
using ChatApp.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                !t.IsAbstract
                && !t.IsInterface
                && t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>)
                    )
            );

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>));

            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, handlerType);
            }
            services.AddScoped(handlerType);
        }
        return services;
    }
}
