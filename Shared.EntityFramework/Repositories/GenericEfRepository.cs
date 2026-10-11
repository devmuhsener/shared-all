using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Domain.Entities;
using Shared.Core.Domain.Repositories;

namespace Shared.EntityFramework.Repositories;

public abstract class GenericEfRepository<TDbContext, TEntity, TId>(TDbContext context) : IRepository<TEntity, TId>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>
{
    protected readonly TDbContext Context = context ?? throw new ArgumentNullException(nameof(context));
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        cancellationToken.ThrowIfCancellationRequested();

        return await DbSet.FindAsync([id], cancellationToken);
    }

    // public async Task<TEntity?> GetByIdNoTrackingAsync(TId id, CancellationToken cancellationToken = default)
    // {
    //     ArgumentNullException.ThrowIfNull(id);
    //     cancellationToken.ThrowIfCancellationRequested();
    //
    //     return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => Equals(id, x.Id), cancellationToken);
    // }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();


        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        cancellationToken.ThrowIfCancellationRequested();

        var entry = await DbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        cancellationToken.ThrowIfCancellationRequested();

        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        DbSet.Remove(entity);
    }

    public virtual async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            Delete(entity);
        }
    }
}