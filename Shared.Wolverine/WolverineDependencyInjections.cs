using System.Reflection;
using ImTools;
using JasperFx;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Core.Infra.Context;
using Shared.Events;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

namespace Shared.Wolverine;

public static class WolverineDependencyInjections
{
    public static IServiceCollection AddSharedWolverine(this IServiceCollection services)
    {
        services.AddScoped<IOutboxCoordinator, OutboxCoordinator>();
        return services;
    }

    public static IHostBuilder UseSharedWolverine(this IHostBuilder builder, string connectionString,
        bool soloMode = false,
        string schemaName = "wolverine", params Assembly[] assemblies)

    {
        builder.UseWolverine(options =>
        {
            // ==========================================
            // 1. GLOBAL / BASE PLATFORM AYARLARI
            // ==========================================
            options.Policies.AddMiddleware(typeof(ExecutionContextWolverineMiddleware));


            if (soloMode)
            {
                options.Durability.Mode = DurabilityMode.Solo;
            }

            options.UseRuntimeCompilation();

            var discovery = options.Discovery
                .DisableConventionalDiscovery()
                .CustomizeHandlerDiscovery(x => { x.Includes.Implements<IMyWolverineHandler>(); })
                .IncludeAssembly(Assembly.GetEntryAssembly()!);
            foreach (var assembly in assemblies)
            {
                discovery.IncludeAssembly(assembly);
            }


            options.PersistMessagesWithPostgresql(connectionString, schemaName);
            options.UseEntityFrameworkCoreTransactions();

            options.CodeGeneration.AlwaysUseServiceLocationFor<IMediator>();
            options.CodeGeneration.AlwaysUseServiceLocationFor<IDomainEventPublisher>();
            options.AutoBuildMessageStorageOnStartup = AutoCreate.CreateOrUpdate;

            options.Policies.AllLocalQueues(q => { q.UseDurableInbox(); });


            // ==========================================
            // 2. MODÜLLERİN ÖZEL AYARLARINI KOŞTURMA
            // ==========================================
            var configuratorTypes = options.Services
                .Where(d => typeof(IWolverineModuleConfigurator).IsAssignableFrom(d.ServiceType))
                .Select(d => d.ImplementationType ?? d.ServiceType)
                .Distinct()
                .ToList();

            foreach (var type in configuratorTypes)
            {
                if (Activator.CreateInstance(type) is IWolverineModuleConfigurator moduleConfigurator)
                {
                    moduleConfigurator.Configure(options);
                }
            }
        });


        return builder;
    }

    public static IServiceCollection AddWolverineConfigurator<TConfigurator>(this IServiceCollection services)
        where TConfigurator : class, IWolverineModuleConfigurator
    {
        services.AddSingleton<IWolverineModuleConfigurator, TConfigurator>();
        return services;
    }
}

public interface IMyWolverineHandler
{
}