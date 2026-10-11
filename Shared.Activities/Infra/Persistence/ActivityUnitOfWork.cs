using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Activities.Domain;
using Shared.Activities.Infra.Persistence.Context;
using Shared.Activities.Infra.Persistence.Repositories;
using Shared.Core.Domain.Events;
using Shared.Events;

namespace Shared.Activities.Infra.Persistence;

public class ActivityUnitOfWork(
    ActivityDbContext context,
    IDomainEventPublisher eventPublisher,
    ILogger<ActivityUnitOfWork> logger) : IActivityUnitOfWork
{
    private const int MaxDomainEventDepth = 10;
    private IUserActivityRepository? _userActivities;

    public IUserActivityRepository Activities
        => _userActivities ??= new UserActivityRepository(context);

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionInternalAsync<object?>(async ct =>
        {
            await action(ct);
            return null;
        }, cancellationToken);
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteInTransactionInternalAsync(action, cancellationToken);
    }

    private async Task<TResult> ExecuteInTransactionInternalAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken ct)
    {
        // 1. Zaten dışarıdan açılmış bir transaction varsa (örn: PlatformUnitOfWork):
        // Yeni transaction açma, mevcut akışa katıl.
        if (context.Database.CurrentTransaction != null)
        {
            var existingResult = await action(ct);
            await context.SaveChangesAsync(ct);
            return existingResult;
        }

        // 2. Transaction yoksa: Fiziksel transaction başlat
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            ct);

        var allDispatchedEvents = new List<IDomainEvent>();

        try
        {
            // 3. Asıl iş mantığını koştur
            var result = await action(ct);

            // 4. CASCADING DRAIN LOOP: Zincirleme domain event kalmayana kadar döngüye sok
            var depth = 0;
            while (true)
            {
                var currentBatch = context.ExtractAndClearDomainEvents();
                if (currentBatch.Count == 0) break;

                depth++;
                if (depth > MaxDomainEventDepth)
                {
                    throw new InvalidOperationException(
                        $"Cascading domain event depth exceeded limit of {MaxDomainEventDepth} in ActivityUnitOfWork.");
                }

                foreach (var domainEvent in currentBatch)
                {
                    await eventPublisher.PublishTransactionalAsync(domainEvent, ct);
                }

                allDispatchedEvents.AddRange(currentBatch);
            }

            // 5. Entity değişikliklerini diske yaz
            await context.SaveChangesAsync(ct);

            // 6. SQL Commit
            await transaction.CommitAsync(ct);

            // 7. Post-Commit Event'ler (Commit sonrası fırlatılan bildirimler)
            foreach (var domainEvent in allDispatchedEvents)
            {
                try
                {
                    await eventPublisher.PublishPostCommitAsync(domainEvent, ct);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while publishing post-commit event: {EventType}",
                        domainEvent.GetType().Name);
                }
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Transaction failed in ActivityUnitOfWork. Rolling back...");
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}