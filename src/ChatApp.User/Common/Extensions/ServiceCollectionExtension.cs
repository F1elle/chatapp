using System.Reflection;
using ChatApp.User.Common.Abstractions;

namespace ChatApp.User.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.Name.EndsWith("Handler") && 
                   !t.IsAbstract && 
                   !t.IsInterface);

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && 
                       i.GetGenericTypeDefinition() == typeof(IHandler<,>));
            
            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, handlerType);
            }

            services.AddScoped(handlerType);
        }

        return services;
    }
}