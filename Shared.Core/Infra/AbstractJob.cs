namespace Shared.Core.Infra;

public abstract record AbstractJob : IBackgroundJob
{
    public Guid Id { get; } = Guid.NewGuid();
}