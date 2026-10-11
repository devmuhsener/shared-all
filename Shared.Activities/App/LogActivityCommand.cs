namespace Shared.Activities.App;

/// <summary>
/// Caller-supplied data for a single activity log entry.
/// Execution context (TraceId, TenantId, ActorId, ActorType, ImpersonatorId, ExecutionType, ClientIp, UserAgent, Device, Country, ClientType)
/// is resolved automatically via <see cref="Shared.Core.Context.IExecutionContextAccessor"/>.
/// </summary>
public record LogActivityCommand(
    string Verb,
    // string? ActorType = null,
    // string? ActorId = null,
    string? OwnerId = null,
    string? ObjectId = null,
    string? ObjectType = null,
    string? TargetId = null,
    string? TargetType = null,
    Dictionary<string, object?>? Metadata = null,
    DateTimeOffset? OccurredAt = null
);