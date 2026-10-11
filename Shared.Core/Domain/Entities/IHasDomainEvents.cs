using System.Collections.ObjectModel;
using Shared.Core.Domain.Events;

namespace Shared.Core.Domain.Entities;

public interface IHasDomainEvents
{
    public IReadOnlyList<IDomainEvent> ReleaseEvents();
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}