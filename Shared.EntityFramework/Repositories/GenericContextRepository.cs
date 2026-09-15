using Microsoft.EntityFrameworkCore;
using Shared.Core.Domain.Entities;
using Shared.Core.Domain.Exceptions;
using Shared.Core.Domain.Repositories;

namespace Shared.EntityFramework.Repositories;

public class GenericContextRepository<TEntity, TKey>(DbContext context) : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly DbContext Context = context ?? throw new ArgumentNullException(nameof(context));
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public virtual async Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to save entity of type {typeof(TEntity).Name}.", ex);
        }
    }

    public virtual async Task SoftDeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new PersistenceException(
                    $"Entity of type {typeof(TEntity).Name} with ID {id} not found for soft delete.");
            }

            await SoftDeleteAsync(entity, cancellationToken);
        }
        catch (PersistenceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to soft delete entity of type {typeof(TEntity).Name} with ID {id}.",
                ex);
        }
    }

    public virtual async Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.Delete();
                DbSet.Update(entity);
            }
            else
            {
                throw new PersistenceException(
                    $"Entity of type {typeof(TEntity).Name} does not implement ISoftDeletable.");
            }
        }
        catch (PersistenceException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to soft delete entity of type {typeof(TEntity).Name}.", ex);
        }
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.FindAsync(new object?[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to retrieve entity of type {typeof(TEntity).Name} with ID {id}.",
                ex);
        }
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to retrieve all entities of type {typeof(TEntity).Name}.", ex);
        }
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            DbSet.Update(entity);
            return Task.FromResult(entity);
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to update entity of type {typeof(TEntity).Name}.", ex);
        }
    }

    public IQueryable<TEntity> Query()
    {
        try
        {
            return DbSet.AsQueryable();
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to query entities of type {typeof(TEntity).Name}.", ex);
        }
    }

    public async Task SaveManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PersistenceException($"Failed to save multiple entities of type {typeof(TEntity).Name}.", ex);
        }
    }
}
