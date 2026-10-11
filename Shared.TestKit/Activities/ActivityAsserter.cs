using Shared.Activities.Domain;
using Shared.Activities.Infra.Persistence.Context;
using Shared.Core.Context;

namespace Shared.TestKit.Activities;

public class ActivityAsserter(IServiceProvider services)
{
    public async Task<UserActivity> ShouldHaveActivityAsync(string verb, ExecutionContextData? expectedContext = null)
    {
        return await services.ExecuteInScopeAsync<ActivityDbContext, UserActivity>(async activityCtx =>
            await activityCtx.ShouldHaveActivityAsync(verb, expectedContext));
    }

    public async Task ShouldNotHaveActivityAsync(string verb)
    {
        await services.ExecuteInScopeAsync<ActivityDbContext>(async activityCtx =>
        {
            await activityCtx.ShouldNotHaveActivityAsync(verb);
        });
    }
}
