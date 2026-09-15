using System.Collections.ObjectModel;

namespace Shared.Core.Domain.Entities;

public abstract class GenericEntity<TId> : IEntity<TId>, IAuditable
{
    private readonly List<IDomainEvent> _domainEvents = [];


    protected GenericEntity()
    {
    }

    protected GenericEntity(TId id)
    {
        Id = id;
        _domainEvents = [];
    }


    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; private set; }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public TId Id { get; } = default!;

    public ReadOnlyCollection<IDomainEvent> ReleaseEvents()
    {
        var events = _domainEvents.AsReadOnly();
        _domainEvents.Clear();
        return events;
    }

    public void RegisterEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}