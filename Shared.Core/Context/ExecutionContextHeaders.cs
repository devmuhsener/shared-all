namespace Shared.Core.Context;

public static class ExecutionContextHeaders
{
    public const string TraceId = "X-Trace-Id";
    public const string TenantId = "X-Tenant-Id";
    public const string ActorId = "X-Actor-Id";
    public const string ActorType = "X-Actor-Type";
    public const string ImpersonatorId = "X-Impersonator-Id";
    public const string ExecutionType = "X-Execution-Type";
    public const string ClientIp = "X-Client-Ip";
    public const string UserAgent = "X-User-Agent";
    public const string Device = "X-Device";
    public const string Country = "X-Country";
    public const string ClientType = "X-Client-Type";
}