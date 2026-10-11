using DotNet.Testcontainers.Builders;
using Testcontainers.Minio;

namespace Shared.DevContainers;

public sealed class MinioTestContainer : IAsyncDisposable
{
    private const int ApiInternalPort = 9000;
    private const int ConsoleInternalPort = 9001;

    private readonly MinioContainer _container;

    public string AccessKey { get; }
    public string SecretKey { get; }

    public MinioTestContainer(
        int? fixedApiPort = null,
        int? fixedConsolePort = null,
        string accessKey = "minioadmin",
        string secretKey = "minioadmin")
    {
        AccessKey = accessKey;
        SecretKey = secretKey;

        var builder = new MinioBuilder("quay.io/minio/minio:RELEASE.2024-03-26T22-10-45Z")
            .WithCommand("--console-address", $":{ConsoleInternalPort}")
            .WithUsername("minioadmin")
            .WithPassword("minioadmin")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPort(ApiInternalPort).ForPath("/minio/health/live")));

        if (fixedApiPort.HasValue)
        {
            builder = builder.WithPortBinding(fixedApiPort.Value, ApiInternalPort);
        }
        else
        {
            builder = builder.WithPortBinding(ApiInternalPort, true);
        }

        if (fixedConsolePort.HasValue)
        {
            builder = builder.WithPortBinding(fixedConsolePort.Value, ConsoleInternalPort);
        }
        else
        {
            builder = builder.WithPortBinding(ConsoleInternalPort, true);
        }

        _container = builder.Build();
    }

    public string Endpoint => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(ApiInternalPort)}";
    public string ConsoleUrl => $"http://{_container.Hostname}:{_container.GetMappedPublicPort(ConsoleInternalPort)}";
    public int MappedApiPort => _container.GetMappedPublicPort(ApiInternalPort);
    public int MappedConsolePort => _container.GetMappedPublicPort(ConsoleInternalPort);

    public Task StartAsync(CancellationToken ct = default) => _container.StartAsync(ct);
    public ValueTask DisposeAsync() => _container.DisposeAsync();
}