using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace Shared.Events;

public delegate Task EventHandlerDelegate(object instance, object @event, CancellationToken cancellationToken);

public static class HandlerInvokerFactory
{
    public static EventHandlerDelegate CreateInvoker(
        Type handlerType,
        MethodInfo method,
        Type eventType
    )
    {
        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var eventParam = Expression.Parameter(typeof(object), "event");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");

        // Handler'ın gerçek tipi her ne ise ona dönüştür. (CASTING)
        // (IdentityOutboxDispatcher)instance
        var typedInstance = Expression.Convert(instanceParam, handlerType);
        // e.g. (UserCreatedEvent)event
        var typedEvent = Expression.Convert(eventParam, eventType);


        var methodParameters = method.GetParameters();
        var arguments = new List<Expression> { typedEvent };

        if (methodParameters.Length > 1 && methodParameters[1].ParameterType == typeof(CancellationToken))
        {
            arguments.Add(ctParam);
        }

        //(IdentityOutboxDispatcher)instance.HandleAsync((UserCreatedEvent)event, ct)
        Expression methodCall = Expression.Call(typedInstance, method, arguments);

        if (method.ReturnType == typeof(void))
        {
            var completedTask = Expression.Constant(Task.CompletedTask);
            methodCall = Expression.Block(methodCall, completedTask);
        }



        var lambda = Expression.Lambda<EventHandlerDelegate>(
            methodCall,
            instanceParam,
            eventParam,
            ctParam
        );

        return lambda.Compile();
    }
}