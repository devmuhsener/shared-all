using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Activities.Domain;
using Shared.Activities.Infra.Persistence.Context;
using Shared.Core.Context;

namespace Shared.TestKit.Activities;

public static class ActivityTestExtensions
{
    public static async Task<UserActivity?> FindActivityAsync(this ActivityDbContext activityCtx, string verb)
    {
        return await activityCtx.Activities
            .FirstOrDefaultAsync(a => a.Verb == verb);
    }

    public static async Task<UserActivity> ShouldHaveActivityAsync(
        this ActivityDbContext activityCtx,
        string verb,
        ExecutionContextData? expectedContext = null,
        CancellationToken cancellationToken = default)
    {
        var query = activityCtx.Activities
            .AsNoTracking()
            .Where(a => a.Verb == verb);

        if (expectedContext?.ActorId != null)
        {
            query = query.Where(a => a.ActorId == expectedContext.ActorId);
        }

        if (expectedContext?.TraceId != null)
        {
            query = query.Where(a => a.TraceId == expectedContext.TraceId);
        }

        var activity = await query
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        activity.Should().NotBeNull(
            because: $"expected an activity with verb '{verb}'" +
                     (expectedContext?.ActorId != null ? $" and ActorId '{expectedContext.ActorId}'" : string.Empty) +
                     (expectedContext?.TraceId != null ? $" and TraceId '{expectedContext.TraceId}'" : string.Empty) +
                     " to be logged in ActivityDbContext");

        if (expectedContext != null)
        {
            activity!.ActorType.Should().Be(expectedContext.ActorType);
        }

        return activity!;
    }

    public static async Task ShouldNotHaveActivityAsync(
        this ActivityDbContext activityCtx,
        string verb,
        ExecutionContextData? expectedContext = null,
        CancellationToken cancellationToken = default)
    {
        var query = activityCtx.Activities
            .AsNoTracking()
            .Where(a => a.Verb == verb);

        if (expectedContext?.ActorId != null)
        {
            query = query.Where(a => a.ActorId == expectedContext.ActorId);
        }

        if (expectedContext?.TraceId != null)
        {
            query = query.Where(a => a.TraceId == expectedContext.TraceId);
        }

        var exists = await query.AnyAsync(cancellationToken);

        exists.Should().BeFalse(
            because: $"expected NO activity with verb '{verb}'" +
                     (expectedContext?.ActorId != null ? $" for ActorId '{expectedContext.ActorId}'" : string.Empty) +
                     (expectedContext?.TraceId != null ? $" and TraceId '{expectedContext.TraceId}'" : string.Empty) +
                     " to be logged in ActivityDbContext");
    }
}