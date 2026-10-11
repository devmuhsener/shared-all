using Shared.Activities.Domain;

namespace Shared.Activities.App;

public interface IActivityLogger
{
    /// <summary>
    /// Logs an activity using a structured command DTO.
    /// Execution context is resolved automatically via IExecutionContextAccessor.
    /// </summary>
    Task<ActivityId> LogAsync(
        LogActivityCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an activity passing each field individually.
    /// Execution context is resolved automatically via IExecutionContextAccessor.
    /// </summary>
    Task<ActivityId> LogAsync(
        string verb,
        // string? actorType = null,
        // string? actorId = null,
        string? ownerId = null,
        string? objectId = null,
        string? objectType = null,
        string? targetId = null,
        string? targetType = null,
        Dictionary<string, object?>? metadata = null,
        DateTimeOffset? occurredAt = null,
        CancellationToken cancellationToken = default);
}