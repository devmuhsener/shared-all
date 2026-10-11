using Shared.Core.Domain.Repositories;

namespace Shared.Activities.Domain;

public interface IUserActivityRepository : IRepository<UserActivity, ActivityId>
{
    Task<IEnumerable<UserActivity>> ListByUserIdAsync(string userId,
        CancellationToken cancellationToken = default);
    
    
}