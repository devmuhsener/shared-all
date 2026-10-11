namespace Shared.DevContainers;

using Testcontainers.PostgreSql;

public sealed class PostgresTestContainer : IAsyncDisposable
{
    private readonly PostgreSqlContainer _container;

    public PostgresTestContainer(string databaseName = "stageup", int? fixedPort = null)
    {
        var builder = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase(databaseName)
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true);

        if (fixedPort.HasValue)
        {
            builder = builder.WithPortBinding(fixedPort.Value, 5432);
        }

        _container = builder.Build();
    }

    public string ConnectionString => _container.GetConnectionString();
    public int Port => _container.GetMappedPublicPort(5432);

    public Task StartAsync(CancellationToken ct = default) => _container.StartAsync(ct);
    public ValueTask DisposeAsync() => _container.DisposeAsync();
}