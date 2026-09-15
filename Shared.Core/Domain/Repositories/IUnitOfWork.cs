namespace Shared.Core.Domain.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}