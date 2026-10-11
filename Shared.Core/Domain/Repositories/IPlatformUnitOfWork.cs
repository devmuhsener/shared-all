namespace Shared.Core.Domain.Repositories;

/// <summary>
/// Provides a cross-context transaction boundary for operations that span
/// multiple DbContexts sharing the same underlying database connection.
///
/// The implementation is responsible for opening a single physical connection,
/// starting a transaction, enlisting all registered DbContexts into it,
/// executing the supplied operation, and committing or rolling back atomically.
/// </summary>
public interface IPlatformUnitOfWork
{
    /// <summary>
    /// Opens a shared database connection, begins a transaction at
    /// <c>ReadCommitted</c> isolation, enlists every registered DbContext
    /// into it, then executes <paramref name="operation"/>.
    /// Commits on success; rolls back and re-throws on any exception.
    /// The connection and transaction are always cleaned up in <c>finally</c>.
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}