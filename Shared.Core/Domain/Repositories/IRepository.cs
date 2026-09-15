using Shared.Core.Domain.Entities;

namespace Shared.Core.Domain.Repositories;

public interface IRepository<TEntity, in TId> where TEntity : IEntity<TId>
{
    public Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task SoftDeleteAsync(TId id, CancellationToken cancellationToken = default);

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public IQueryable<TEntity> Query();

    public Task SaveManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}