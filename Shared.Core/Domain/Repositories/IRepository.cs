using Shared.Core.Domain.Entities;

namespace Shared.Core.Domain.Repositories;

public interface IRepository<TEntity, in TId> where TEntity : class, IEntity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    // Task<TEntity?> GetByIdNoTrackingAsync(TId id, CancellationToken cancellationToken = default);


    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);
}