namespace Shared.Core.Context;

public interface IExecutionContextAccessor
{
    ExecutionContextData Context { get; }

    void SetContext(ExecutionContextData context);
    
    void RefreshContext(string actorId, string actorType);
}