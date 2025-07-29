using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nama.Internal;

namespace Nama;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNama(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.AddTransient<ISender, Sender>();
        services.AddTransient<IPublisher, Publisher>();
        
        var types = assembly.GetTypes();
        
        var handlers = types
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Select(type => new
            {
                ImplementationType = type,
                InterfaceTypes = type
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .ToList()
            })
            .Where(entry => entry.InterfaceTypes.Count > 0)
            .ToList();

        foreach (var handler in handlers)
        {
            foreach (var interfaceType in handler.InterfaceTypes)
            {
                services.AddScoped(interfaceType, handler.ImplementationType);
            }
        }
        
        var notificationHandlers = types
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Select(type => new
            {
                ImplementationType = type,
                InterfaceTypes = type
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                    .ToList()
            })
            .Where(entry => entry.InterfaceTypes.Count > 0)
            .ToList();

        foreach (var handler in notificationHandlers)
        {
            foreach (var interfaceType in handler.InterfaceTypes)
            {
                services.AddScoped(interfaceType, handler.ImplementationType);
            }
        }
        
        return services;
    }
}