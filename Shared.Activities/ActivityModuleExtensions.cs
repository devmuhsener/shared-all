using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Shared.Activities.App;
using Shared.Activities.Domain;
using Shared.Activities.Infra.Persistence;
using Shared.Activities.Infra.Persistence.Context;
using Shared.Activities.Infra.Persistence.Repositories;

namespace Shared.Activities;

public static class ActivityModuleExtensions
{
    public static IServiceCollection AddActivityContext(this IServiceCollection services, DbDataSource dataSource)
    {
        services.AddDbContextPool<ActivityDbContext>(options =>
        {
            ActivityDbContextFactory.ConfigureOptions(dataSource, options);
        });

        return services;
    }

    public static IServiceCollection AddActivityModule(this IServiceCollection services)
    {
        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<IActivityUnitOfWork, ActivityUnitOfWork>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<IUserActivityQueryRepository, ActivityQueryRepository>();


        return services;
    }
}