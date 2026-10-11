using Shared.Core.Context;
using Wolverine;

namespace Shared.Wolverine;

public static class ExecutionContextDeliveryOptionsExtensions
{
    public static DeliveryOptions ToDeliveryOptions(
        this IExecutionContextAccessor accessor,
        string? executionTypeOverride = "background_job")
    {
        return accessor.Context.ToDeliveryOptions(executionTypeOverride);
    }

    public static DeliveryOptions ToDeliveryOptions(
        this ExecutionContextData ctx,
        string? executionTypeOverride = "background_job")
    {
        var options = new DeliveryOptions();


        // 1. Dağıtık İzleme
        if (!string.IsNullOrWhiteSpace(ctx.TraceId))
            options.Headers[ExecutionContextHeaders.TraceId] = ctx.TraceId;

        // 2. Mülkiyet & Aktör
        if (!string.IsNullOrWhiteSpace(ctx.TenantId))
            options.Headers[ExecutionContextHeaders.TenantId] = ctx.TenantId;

        if (!string.IsNullOrWhiteSpace(ctx.ActorId))
            options.Headers[ExecutionContextHeaders.ActorId] = ctx.ActorId;

        options.Headers[ExecutionContextHeaders.ActorType] = ctx.ActorType;

        if (!string.IsNullOrWhiteSpace(ctx.ImpersonatorId))
            options.Headers[ExecutionContextHeaders.ImpersonatorId] = ctx.ImpersonatorId;

        // 3. İcra Türü (Default olarak kuyruğa girdiği için 'background_job')
        options.Headers[ExecutionContextHeaders.ExecutionType] = executionTypeOverride ?? ctx.ExecutionType;

        // 4. İstemci & Çevre Verileri
        if (!string.IsNullOrWhiteSpace(ctx.ClientIp))
            options.Headers[ExecutionContextHeaders.ClientIp] = ctx.ClientIp;

        if (!string.IsNullOrWhiteSpace(ctx.UserAgent))
            options.Headers[ExecutionContextHeaders.UserAgent] = ctx.UserAgent;

        if (!string.IsNullOrWhiteSpace(ctx.Device))
            options.Headers[ExecutionContextHeaders.Device] = ctx.Device;

        if (!string.IsNullOrWhiteSpace(ctx.Country))
            options.Headers[ExecutionContextHeaders.Country] = ctx.Country;

        if (!string.IsNullOrWhiteSpace(ctx.ClientType))
            options.Headers[ExecutionContextHeaders.ClientType] = ctx.ClientType;

        return options;
    }
}