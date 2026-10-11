using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Shared.EntityFramework.Extensions;

public static class SharedEfDatasourceFactory
{
    public static NpgsqlDataSource CreateNpgsqlDataSource(string? connectionString)
    {
        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        return new NpgsqlDataSourceBuilder(connectionString)
            .EnableDynamicJson()
            .Build();
    }
}