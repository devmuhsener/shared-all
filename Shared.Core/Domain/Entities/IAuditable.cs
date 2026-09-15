namespace Shared.Core.Domain.Entities;

public interface IAuditable
{
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? UpdatedAt { get; }

    public void MarkAsUpdated();
}