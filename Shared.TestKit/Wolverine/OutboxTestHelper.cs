using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Shared.Core.Context;
using Wolverine;
using Wolverine.Persistence.Durability;
using Wolverine.Tracking;

namespace Shared.TestKit.Wolverine;

public class OutboxTestHelper(IServiceProvider services, IHost host)
{
    public async Task<Envelope> ShouldHaveIncomingMessageAsync<TMessage>(ExecutionContextData? expectedContext = null)
    {
        return await services.ExecuteInScopeAsync<IMessageStore, Envelope>(async store =>
            await store.ShouldHaveIncomingMessageAsync<TMessage>(expectedContext));
    }


    public async Task ShouldNotHaveIncomingMessageAsync<TMessage>()
    {
        await services.ExecuteInScopeAsync<IMessageStore>(async store =>
        {
            await store.ShouldNotHaveIncomingMessageAsync<TMessage>();
        });
    }

    public Task<ITrackedSession> PublishJobAndWaitAsync<TDbContext>(
        object job,
        ExecutionContextData? contextData = null) where TDbContext : DbContext
    {
        return host.EnqueueJobAndWaitAsync<TDbContext>(job, contextData);
    }
}