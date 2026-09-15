using Microsoft.EntityFrameworkCore;
using Shared.Core.Domain.Exceptions;
using Shared.Core.Domain.Repositories;

namespace Shared.EntityFramework;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    private readonly DbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new PersistenceException("An error occurred while saving changes to the context.", ex);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}