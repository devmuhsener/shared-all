using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Context;
using Shared.TestKit.Context;
using Wolverine;

namespace Shared.TestKit;

public static class ServiceProviderScopeExtensions
{
    private static void InjectContext(IServiceProvider sp, Action<ExecutionOptionsBuilder>? configure = null)
    {
        if (configure == null) return;
        var builder = new ExecutionOptionsBuilder();
        configure(builder);
        var contextData = builder.Build().ContextData;
        sp.GetRequiredService<IExecutionContextAccessor>()
            .SetContext(contextData);
    }


    public static async Task ExecuteInScopeAsync<TService>(this IServiceProvider sp, Func<TService, Task> action,
        Action<ExecutionOptionsBuilder>? config = null)
        where TService : notnull
    {
        await using var scope = sp.CreateAsyncScope();

        InjectContext(scope.ServiceProvider, config);

        var service = scope.ServiceProvider.GetRequiredService<TService>();
        await action(service);
    }

    public static async Task ExecuteInScopeAsync(this IServiceProvider sp, Func<IServiceProvider, Task> action,
        Action<ExecutionOptionsBuilder>? config = null)
    {
        await using var scope = sp.CreateAsyncScope();
        InjectContext(scope.ServiceProvider, config);
        await action(scope.ServiceProvider);
    }

    public static async Task<TResult> ExecuteInScopeAsync<TService, TResult>(this IServiceProvider sp,
        Func<TService, Task<TResult>> action, Action<ExecutionOptionsBuilder>? config = null)
        where TService : notnull
    {
        await using var scope = sp.CreateAsyncScope();
        InjectContext(scope.ServiceProvider, config);
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        return await action(service);
    }
}

public record ExecutionOptions(ExecutionContextData ContextData);

public class ExecutionOptionsBuilder
{
    private ExecutionContextData _contextData = ExecutionContextBuilder.AnExecutionContextData().Build();


    public ExecutionOptionsBuilder? WithContextData(ExecutionContextData? contextData)
    {
        if (contextData == null) return null;
        
        _contextData = contextData;
        return this;
    }

    public ExecutionOptions Build()
    {
        return new ExecutionOptions(_contextData);
    }
}