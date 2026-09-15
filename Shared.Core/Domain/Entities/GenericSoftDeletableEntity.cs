namespace Shared.Core.Domain.Entities;

public class GenericSoftDeletableEntity<TId> : GenericEntity<TId>, ISoftDeletable
{
    protected GenericSoftDeletableEntity()
    {
    }

    protected GenericSoftDeletableEntity(TId id) : base(id)
    {
    }

    public DateTimeOffset? DeletedAt { get; private set; }

    public virtual void Delete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public bool IsDeleted()
    {
        return DeletedAt != null;
    }
}