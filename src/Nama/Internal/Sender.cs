using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Nama.Internal;

internal sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    private static readonly ConcurrentDictionary<Type, HandlerWrapper> HandlerWrapperCache = new();
    
    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();

        var handlerWrapper = HandlerWrapperCache.GetOrAdd(requestType, CreateHandlerWrapper);
        
        using var scope = serviceProvider.CreateScope();

        var result = await handlerWrapper.HandleAsync(request, scope, cancellationToken);

        return (TResponse)result!;
    }

    private static HandlerWrapper CreateHandlerWrapper(Type requestType)
    {
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            ?? throw new InvalidOperationException($"Request type '{requestType.Name}' does not implement IRequest<TResponse>.");
        
        var responseType = requestInterface.GetGenericArguments()[0];
        
        var wrapperType = typeof(HandlerWrapper<,>).MakeGenericType(requestType, responseType);
        
        return (HandlerWrapper)Activator.CreateInstance(wrapperType)!;
    }
    
    private abstract class HandlerWrapper
    {
        public abstract Task<object?> HandleAsync(
            object request,
            IServiceScope scope,
            CancellationToken cancellationToken);
    }
    
    private sealed class HandlerWrapper<TRequest, TResponse> : HandlerWrapper
        where TRequest : IRequest<TResponse>
    {
        public override async Task<object?> HandleAsync(
            object request,
            IServiceScope scope,
            CancellationToken cancellationToken)
        {
            var handler = scope.ServiceProvider.GetService<IRequestHandler<TRequest, TResponse>>();

            if (handler is null)
            {
                throw new InvalidOperationException(
                    $"No handler registered for request type '{typeof(TRequest).Name}'");
            }

            return await handler.Handle((TRequest)request, cancellationToken);
        }
    }
}