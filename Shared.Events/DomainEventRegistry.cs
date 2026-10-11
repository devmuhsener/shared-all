using System.ComponentModel;
using Shared.Core.Domain.Events;

namespace Shared.Events;

public class DomainEventRegistry
{
    private readonly Dictionary<Type, List<HandlerRegistration>> _transactionalHandlers = new();
    private readonly Dictionary<Type, List<HandlerRegistration>> _postCommitHandlers = new();

    public void RegisterTransactional(Type eventType, Type handlerType, EventHandlerDelegate invoker)
    {
        if (!_transactionalHandlers.ContainsKey(eventType))
        {
            _transactionalHandlers[eventType] = [];
        }

        _transactionalHandlers[eventType].Add(new HandlerRegistration(handlerType, invoker));
    }

    public void RegisterPostCommit(Type eventType, Type handlerType, EventHandlerDelegate invoker)
    {
        if (!_postCommitHandlers.ContainsKey(eventType))
        {
            _postCommitHandlers[eventType] = [];
        }

        _postCommitHandlers[eventType].Add(new HandlerRegistration(handlerType, invoker));
    }

    public IEnumerable<HandlerRegistration> GetTransactionalHandlers(Type eventType)
    {
        return GetMatchingHandlers(_transactionalHandlers, eventType);
    }

    public IEnumerable<HandlerRegistration> GetPostCommitHandlers(Type eventType)
    {
        return GetMatchingHandlers(_postCommitHandlers, eventType);
    }


    private static IEnumerable<HandlerRegistration> GetMatchingHandlers(
        Dictionary<Type, List<HandlerRegistration>> handlersDict,
        Type eventType)
    {
        if (handlersDict.TryGetValue(eventType, out var directHandlers))
        {
            foreach (var handler in directHandlers)
                yield return handler;
        }


        foreach (var @interface in eventType.GetInterfaces())
        {
            if (handlersDict.TryGetValue(@interface, out var interfaceHandlers))
            {
                foreach (var handler in interfaceHandlers)
                    yield return handler;
            }
        }


        var baseType = eventType.BaseType;
        while (baseType != null && baseType != typeof(object))
        {
            if (handlersDict.TryGetValue(baseType, out var baseHandlers))
            {
                foreach (var handler in baseHandlers)
                    yield return handler;
            }

            baseType = baseType.BaseType;
        }
    }

    public void RegisterMock<TEvent>(Func<TEvent, CancellationToken, Task> handlerAction, bool isTransactional)
        where TEvent : IDomainEvent
    {
        EventHandlerDelegate invoker = async (instance, @event, ct) => { await handlerAction((TEvent)@event, ct); };

        var registration = new HandlerRegistration(typeof(object), invoker);

        var targetDict = isTransactional ? _transactionalHandlers : _postCommitHandlers;
        var eventType = typeof(TEvent);

        if (!targetDict.ContainsKey(eventType))
        {
            targetDict[eventType] = new List<HandlerRegistration>();
        }

        targetDict[eventType].Add(registration);
    }
}

public record HandlerRegistration(
    Type HandlerType,
    EventHandlerDelegate Invoker
);