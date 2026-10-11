using FastExpressionCompiler.ILDecoder;
using Wolverine;

namespace Shared.Wolverine;

public interface IOutboxCoordinator
{
    Task PublishAsync<T>(T message) where T : class;

    IDisposable Enlist(Func<object, Task> publishAction);
}

public class OutboxCoordinator(IMessageBus fallbackBus) : IOutboxCoordinator
{
    private readonly Stack<Func<object, Task>> _scopes = new();


    public async Task PublishAsync<T>(T message) where T : class
    {
        if (_scopes.TryPeek(out var currentPublishAction))
        {
            await currentPublishAction(message);
            return;
        }

        await fallbackBus.PublishAsync(message);
    }

    public IDisposable Enlist(Func<object, Task> publishAction)
    {
        ArgumentNullException.ThrowIfNull(publishAction);

        _scopes.Push(publishAction);
        return new ScopeToken(this);
    }


    private void Pop()
    {
        if (_scopes.Count > 0)
        {
            _scopes.Pop();
        }
    }


    private sealed class ScopeToken(OutboxCoordinator coordinator) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            coordinator.Pop();
            _disposed = true;
        }
    }
}