using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Shared.Activities.Infra.Persistence.Context;

namespace Shared.Activities.Infra.Persistence.Initialization;

public static class ActivityDatabaseInitializer
{
    public static async Task MigrateAsync(DbDataSource dataSource, CancellationToken cancellationToken = default)
    {
        await using var context = ActivityDbContextFactory.Create(dataSource);

        await context.Database.MigrateAsync(cancellationToken);
    }
}