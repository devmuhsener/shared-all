using Shared.Core.Context;

namespace Shared.Core.Infra.Context;

public class ExecutionContextAccessor : IExecutionContextAccessor
{
    private static ExecutionContextData? _currentContext;

    public ExecutionContextData Context =>
        _currentContext ?? ExecutionContextData.System();

    public void SetContext(ExecutionContextData context)
    {
        _currentContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void RefreshContext(string actorId, string actorType)
    {
        if (_currentContext == null)
        {
            SetContext(ExecutionContextData.System() with { ActorId = actorId, ActorType = actorType });
        }
        else
        {
            SetContext(_currentContext with { ActorId = actorId, ActorType = actorType });
        }
    }
}