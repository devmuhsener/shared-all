using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Shared.Activities.Domain;
using Shared.Core.Context;

namespace Shared.Activities.App;

public sealed class ActivityLogger(
    IActivityUnitOfWork unitOfWork,
    ILogger<ActivityLogger> logger,
    IExecutionContextAccessor contextAccessor) : IActivityLogger
{
    // ── Public API ───────────────────────────────────────────────────────────

    public Task<ActivityId> LogAsync(
        LogActivityCommand command,
        CancellationToken cancellationToken = default)
        => LogAsync(
            verb: command.Verb,
            // actorType: command.ActorType,
            // actorId: command.ActorId,
            ownerId: command.OwnerId,
            objectId: command.ObjectId,
            objectType: command.ObjectType,
            targetId: command.TargetId,
            targetType: command.TargetType,
            metadata: command.Metadata,
            occurredAt: command.OccurredAt,
            cancellationToken: cancellationToken);

    public async Task<ActivityId> LogAsync(
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
        CancellationToken cancellationToken = default)
    {
        var ctx = contextAccessor.Context;

        // var effectiveActorType = !string.IsNullOrWhiteSpace(actorType) ? actorType : ctx.ActorType;
        // var effectiveActorId = !string.IsNullOrWhiteSpace(actorId) ? actorId : ctx.ActorId;
        var effectiveActorType = ctx.ActorType;
        var effectiveActorId = ctx.ActorId;

        JsonDocument? jsonMetadata = null;
        if (metadata is { Count: > 0 })
        {
            var json = JsonSerializer.Serialize(metadata);
            jsonMetadata = JsonDocument.Parse(json);
        }

        var activity = UserActivity.Create(
            actorType: effectiveActorType,
            verb: verb,
            actorId: effectiveActorId,
            ownerId: ownerId,
            traceId: ctx.TraceId,
            tenantId: ctx.TenantId,
            impersonatorId: ctx.ImpersonatorId,
            executionType: ctx.ExecutionType,
            objectId: objectId,
            objectType: objectType,
            targetId: targetId,
            targetType: targetType,
            metadata: jsonMetadata,
            clientIp: ctx.ClientIp,
            userAgent: ctx.UserAgent,
            device: ctx.Device,
            country: ctx.Country,
            clientType: ctx.ClientType,
            occurredAt: occurredAt);

        await unitOfWork.ExecuteInTransactionAsync(
            async ct => { await unitOfWork.Activities.AddAsync(activity, ct); },
            cancellationToken);

        logger.LogInformation(
            "Logged: {Verb}, TraceId: {TraceId}, OwnerId: {OwnerId},ActorId: {ActorId}, ActorType: {ActorType}", verb,
            ctx.TraceId, ownerId, ctx.ActorId, ctx.ActorType);


        return activity.Id;
    }
}