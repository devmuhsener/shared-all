namespace Shared.Core.Domain.Entities;

public abstract class BaseEntity : GenericEntity<Guid>
{
    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid id) : base(id)
    {
    }
}