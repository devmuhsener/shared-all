namespace Shared.Core.Context;

public sealed record ExecutionContextData(
    // Tracing
    string? TraceId,

    // Actor-related fields
    string? TenantId,
    string? ActorId,
    string ActorType, // "user", "system", "guest", "admin"
    string? ImpersonatorId,
    string ExecutionType, // "interactive", "background_job", "scheduled_job"

    // Environment fields
    string? ClientIp,
    string? UserAgent,
    string? Device,
    string? Country,
    string? ClientType // "Web", "Mobile", "Worker"
)
{
    public static ExecutionContextData System(string? traceId = null, string executionType = "scheduled_job")
        => new(
            TraceId: traceId,
            TenantId: null,
            ActorId: null,
            ActorType: "system",
            ImpersonatorId: null,
            ExecutionType: executionType,
            ClientIp: null,
            UserAgent: null,
            Device: null,
            Country: null,
            ClientType: "Worker"
        );

    public static ExecutionContextData User(string actorId, string? traceId = null,
        string executionType = "interactive")
        => new(
            TraceId: traceId,
            TenantId: null,
            ActorId: actorId,
            ActorType: "User",
            ImpersonatorId: null,
            ExecutionType: executionType,
            ClientIp: null,
            UserAgent: null,
            Device: null,
            Country: null,
            ClientType: "Worker"
        );
}