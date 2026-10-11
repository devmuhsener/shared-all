using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Shared.DevContainers;

public sealed class MailDevTestContainer : IAsyncDisposable
{
    private const int SmtpInternalPort = 1025;
    private const int WebUiInternalPort = 1080;

    private readonly IContainer _container;

    public MailDevTestContainer(int? fixedWebPort = null, int? fixedSmtpPort = null)
    {
        var builder = new ContainerBuilder()
            .WithImage("maildev/maildev:latest")
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(WebUiInternalPort).ForPath("/")));

        if (fixedSmtpPort.HasValue)
        {
            builder = builder.WithPortBinding(fixedSmtpPort.Value, SmtpInternalPort);
        }
        else
        {
            builder = builder.WithPortBinding(SmtpInternalPort, true);
        }

        if (fixedWebPort.HasValue)
        {
            builder = builder.WithPortBinding(fixedWebPort.Value, WebUiInternalPort);
        }
        else
        {
            builder = builder.WithPortBinding(WebUiInternalPort, true);
        }

        _container = builder.Build();
    }

    public string Host => _container.Hostname;
    public int MappedSmtpPort => _container.GetMappedPublicPort(SmtpInternalPort);
    public int MappedWebPort => _container.GetMappedPublicPort(WebUiInternalPort);
    public string WebUiUrl => $"http://{_container.Hostname}:{MappedWebPort}";

    public Task StartAsync(CancellationToken ct = default) => _container.StartAsync(ct);
    public ValueTask DisposeAsync() => _container.DisposeAsync();
}