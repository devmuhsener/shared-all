namespace Shared.Core.Domain.Entities;

public interface IDomainEvent
{
    public DateTimeOffset OccurredAt { get; }
}