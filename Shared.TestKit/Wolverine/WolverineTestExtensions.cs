using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Shared.Core.Context;
using Shared.Wolverine;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Tracking;

namespace Shared.TestKit.Wolverine;

public static class WolverineTestExtensions
{
    public static Task EnqueueJobAndWaitAsync<TJob, TDbContext>(
        this IHost host,
        TJob job,
        ExecutionContextData? contextData = null)
        where TJob : notnull
        where TDbContext : DbContext
    {
        var deliveryOptions = contextData?.ToDeliveryOptions() ?? new DeliveryOptions();

        return host.ExecuteAndWaitAsync(async ctx =>
        {
            await host.Services.ExecuteInScopeAsync<IDbContextOutbox<TDbContext>>(async outbox =>
            {
                await outbox.PublishAsync(job, deliveryOptions);
                await outbox.SaveChangesAndFlushMessagesAsync();
            });
        });
    }

    public static Task<ITrackedSession> EnqueueJobAndWaitAsync<TDbContext>(
        this IHost host,
        object job,
        ExecutionContextData? contextData = null)
        where TDbContext : DbContext
    {
        var deliveryOptions = contextData?.ToDeliveryOptions() ?? new DeliveryOptions();

        return host.ExecuteAndWaitAsync(async ctx =>
        {
            await host.Services.ExecuteInScopeAsync<IDbContextOutbox<TDbContext>>(async outbox =>
            {
                await outbox.PublishAsync(job, deliveryOptions);
                await outbox.SaveChangesAndFlushMessagesAsync();
            });
        });
    }
}