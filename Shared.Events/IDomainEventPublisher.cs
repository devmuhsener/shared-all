using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Domain.Events;

namespace Shared.Events;

public interface IDomainEventPublisher
{
    Task PublishTransactionalAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
    Task PublishPostCommitAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
}

public class DomainEventPublisher(
    IServiceProvider serviceProvider,
    DomainEventRegistry registry) : IDomainEventPublisher
{
    public async Task PublishTransactionalAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var handlers = registry.GetTransactionalHandlers(@event.GetType());

        foreach (var registration in handlers)
        {
            var handlerInstance = registration.HandlerType == typeof(object)
                ? new object()
                : serviceProvider.GetRequiredService(registration.HandlerType);

            await registration.Invoker(handlerInstance, @event, cancellationToken);
        }
    }

    public async Task PublishPostCommitAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var handlers = registry.GetPostCommitHandlers(@event.GetType());

        foreach (var registration in handlers)
        {
            var handlerInstance = registration.HandlerType == typeof(object)
                ? new object()
                : serviceProvider.GetRequiredService(registration.HandlerType);

            await registration.Invoker(handlerInstance, @event, cancellationToken);
        }
    }
}