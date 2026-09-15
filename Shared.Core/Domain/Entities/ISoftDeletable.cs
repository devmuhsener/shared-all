namespace Shared.Core.Domain.Entities;

public interface ISoftDeletable
{
    public DateTimeOffset? DeletedAt { get; }

    public bool IsDeleted();

    public void Delete();
}