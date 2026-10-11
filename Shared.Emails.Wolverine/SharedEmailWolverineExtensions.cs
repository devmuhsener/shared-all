using JasperFx.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Wolverine;
using Wolverine;
using Wolverine.ErrorHandling;

namespace Shared.Emails.Wolverine;

public static class SharedEmailWolverineExtensions
{
    public static IServiceCollection UseWolverineEmailQueue(this IServiceCollection services)
    {
        services.AddScoped<IEmailQueue, WolverineEmailQueue>();

        // Wolverine
        services.AddSingleton<SharedEmailWolverineConfigurator>();
        return services;
    }
}

public class SharedEmailWolverineConfigurator : IWolverineModuleConfigurator
{
    public void Configure(WolverineOptions options)
    {
        options.Discovery.IncludeAssembly(typeof(SharedEmailWolverineExtensions).Assembly);
    }
}