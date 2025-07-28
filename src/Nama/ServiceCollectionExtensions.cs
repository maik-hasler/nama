using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nama.Internal;

namespace Nama;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddName(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.AddTransient<ISender, Sender>();
        
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
        
        return services;
    }
}