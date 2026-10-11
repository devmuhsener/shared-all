using Shared.Core.Context;
using Wolverine;

namespace Shared.Wolverine;

public static class ExecutionContextWolverineMiddleware
{
    public static void Before(Envelope envelope, IExecutionContextAccessor contextAccessor)
    {
        var headers = envelope.Headers;


        var traceId = headers.TryGetValue(ExecutionContextHeaders.TraceId, out var tid)
            ? tid
            : envelope.CorrelationId;

        headers.TryGetValue(ExecutionContextHeaders.TenantId, out var tenantId);
        headers.TryGetValue(ExecutionContextHeaders.ActorId, out var actorId);

        var actorType = headers.TryGetValue(ExecutionContextHeaders.ActorType, out var at) &&
                        !string.IsNullOrWhiteSpace(at)
            ? at
            : "system";

        headers.TryGetValue(ExecutionContextHeaders.ImpersonatorId, out var impersonatorId);

        var executionType = headers.TryGetValue(ExecutionContextHeaders.ExecutionType, out var et) &&
                            !string.IsNullOrWhiteSpace(et)
            ? et
            : "background_job";

        headers.TryGetValue(ExecutionContextHeaders.ClientIp, out var clientIp);
        headers.TryGetValue(ExecutionContextHeaders.UserAgent, out var userAgent);
        headers.TryGetValue(ExecutionContextHeaders.Device, out var device);
        headers.TryGetValue(ExecutionContextHeaders.Country, out var country);
        headers.TryGetValue(ExecutionContextHeaders.ClientType, out var clientType);

        var contextData = new ExecutionContextData(
            TraceId: traceId,
            TenantId: tenantId,
            ActorId: actorId,
            ActorType: actorType,
            ImpersonatorId: impersonatorId,
            ExecutionType: executionType,
            ClientIp: clientIp,
            UserAgent: userAgent,
            Device: device,
            Country: country,
            ClientType: clientType ?? "Worker"
        );

        // O anki scope'a yükle!
        contextAccessor.SetContext(contextData);
    }
}