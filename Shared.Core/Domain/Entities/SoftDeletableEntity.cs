namespace Shared.Core.Domain.Entities;

public abstract class SoftDeletableEntity : GenericSoftDeletableEntity<Guid>
{
    protected SoftDeletableEntity()
    {
    }

    public SoftDeletableEntity(Guid id) : base(id)
    {
    }
}