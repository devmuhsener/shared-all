using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Context;
using Shared.Core.Infra.Context;

namespace Shared.Core;

public static class SharedCoreExtensions
{
    public static IServiceCollection AddSharedCoreModule(this IServiceCollection services)
    {
        services.AddScoped<IExecutionContextAccessor, ExecutionContextAccessor>();

        return services;
    }
}