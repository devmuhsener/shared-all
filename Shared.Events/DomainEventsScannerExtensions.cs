using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Core.Domain.Events;

namespace Shared.Events;

public static class DomainEventsScannerExtensions
{
    public static IServiceCollection AddDomainEventHandlersFromAssembly(this IServiceCollection services,
        Assembly assembly)
    {
        var registry = AddDomainEventRegistry(services);

        var candidateTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false });


        foreach (var type in candidateTypes)
        {
            var isTransactional = typeof(ITransactionalEventHandler).IsAssignableFrom(type);
            var isPostCommit = typeof(IPostCommitEventHandler).IsAssignableFrom(type);

            if (!isTransactional && !isPostCommit)
                continue;

            // This class is a handler! Add it to DI as scoped.
            // Use TryAddScoped because we want it to be instantiated only once in a request scope.
            services.TryAddScoped(type);

            var methods = type.GetMethods(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.Name is "Handle" or "HandleAsync");

            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length == 0) continue;

                var eventType = parameters[0].ParameterType;
                if (!typeof(IDomainEvent).IsAssignableFrom(eventType)) continue;


                var invoker = HandlerInvokerFactory.CreateInvoker(type, methodInfo, eventType);

                if (isTransactional)
                {
                    registry.RegisterTransactional(eventType, handlerType: type, invoker);
                }

                if (isPostCommit)
                {
                    registry.RegisterPostCommit(eventType, handlerType: type, invoker);
                }
            }
        }

        services.TryAddScoped<IDomainEventPublisher, DomainEventPublisher>();

        return services;
    }


    private static DomainEventRegistry AddDomainEventRegistry(IServiceCollection services)
    {
        var registry = services
            .FirstOrDefault(d => d.ServiceType == typeof(DomainEventRegistry))
            ?.ImplementationInstance as DomainEventRegistry;

        if (registry != null) return registry;

        registry = new DomainEventRegistry();
        services.AddSingleton(registry);

        return registry;
    }
}