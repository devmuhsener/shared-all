using Microsoft.EntityFrameworkCore;
using Shared.Core.Domain.Entities;
using Shared.Core.Domain.Events;

namespace Shared.Events;

public static class DbContextDomainEventExtensions
{
    public static List<IDomainEvent> ExtractAndClearDomainEvents(this DbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entity => entity.ReleaseEvents()) // It gets and clears
            .ToList();


        return domainEvents;
    }
}