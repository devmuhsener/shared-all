using Microsoft.EntityFrameworkCore;
using Shared.Activities.Domain;
using Shared.Activities.Infra.Persistence.Context;
using Shared.EntityFramework.Repositories;

namespace Shared.Activities.Infra.Persistence.Repositories;

public class UserActivityRepository(ActivityDbContext context)
    : GenericEfRepository<ActivityDbContext, UserActivity, ActivityId>(context), IUserActivityRepository
{
    public async Task<IEnumerable<UserActivity>> ListByUserIdAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Activities
            .Where(x => x.ActorId == userId)
            .ToListAsync(cancellationToken);
    }
}