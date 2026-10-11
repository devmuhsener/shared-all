using System.Net;
using System.Text.Json;
using Shared.Core.Domain.Entities;
using Shared.Core.Domain.Exceptions;

namespace Shared.Activities.Domain;

public class UserActivity : IEntity<ActivityId>
{
    private UserActivity()
    {
    }

    private UserActivity(
        ActivityId id,
        string? traceId,
        string? tenantId,
        string? actorId,
        string actorType,
        string? ownerId,
        string? impersonatorId,
        string? executionType,
        string verb,
        string? objectId,
        string? objectType,
        string? targetId,
        string? targetType,
        JsonDocument? metadata,
        string? clientIp,
        string? userAgent,
        string? device,
        string? country,
        string? clientType,
        DateTimeOffset occurredAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        TraceId = traceId;
        TenantId = tenantId;
        ActorId = actorId;
        ActorType = actorType;
        OwnerId = ownerId;
        ImpersonatorId = impersonatorId;
        ExecutionType = executionType;
        Verb = verb;
        ObjectId = objectId;
        ObjectType = objectType;
        TargetId = targetId;
        TargetType = targetType;
        Metadata = metadata;
        UserAgent = userAgent;
        Device = device;
        Country = country;
        ClientType = clientType;
        OccurredAt = occurredAt;
        CreatedAt = createdAt;

        ClientIp = IPAddress.TryParse(clientIp, out var clientIpAddress) ? clientIpAddress : null;
    }

    public ActivityId Id { get; private set; } = default!;
    public string? TraceId { get; private set; }
    public string? TenantId { get; private set; }
    public string? ActorId { get; private set; }
    public string ActorType { get; private set; } = default!;
    public string? OwnerId { get; private set; }
    public string? ImpersonatorId { get; private set; }
    public string? ExecutionType { get; private set; }
    public string Verb { get; private set; } = default!;
    public string? ObjectId { get; private set; }
    public string? ObjectType { get; private set; }
    public string? TargetId { get; private set; }
    public string? TargetType { get; private set; }
    public JsonDocument? Metadata { get; private set; }
    public IPAddress? ClientIp { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Device { get; private set; }
    public string? Country { get; private set; }
    public string? ClientType { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static UserActivity Create(
        string actorType,
        string verb,
        string? actorId = null,
        string? ownerId = null,
        string? traceId = null,
        string? tenantId = null,
        string? impersonatorId = null,
        string? executionType = null,
        string? objectId = null,
        string? objectType = null,
        string? targetId = null,
        string? targetType = null,
        JsonDocument? metadata = null,
        string? clientIp = null,
        string? userAgent = null,
        string? device = null,
        string? country = null,
        string? clientType = null,
        DateTimeOffset? occurredAt = null)
    {
        var validation = Validate(actorType, actorId, verb);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var normalizedActorType = actorType.Trim().ToLowerInvariant();
        var normalizedExecutionType = string.IsNullOrWhiteSpace(executionType) ? "interactive" : executionType.Trim();
        var trimmedUserAgent = userAgent?.Trim();
        if (trimmedUserAgent is { Length: > 500 })
        {
            trimmedUserAgent = trimmedUserAgent[..500];
        }

        var now = DateTimeOffset.UtcNow;

        return new UserActivity(
            id: ActivityId.New(),
            traceId: traceId?.Trim(),
            tenantId: tenantId?.Trim(),
            actorId: actorId?.Trim(),
            actorType: normalizedActorType,
            ownerId: ownerId?.Trim(),
            impersonatorId: impersonatorId?.Trim(),
            executionType: normalizedExecutionType,
            verb: verb.Trim(),
            objectId: objectId?.Trim(),
            objectType: objectType?.Trim(),
            targetId: targetId?.Trim(),
            targetType: targetType?.Trim(),
            metadata: metadata,
            clientIp: clientIp?.Trim(),
            userAgent: trimmedUserAgent,
            device: device?.Trim(),
            country: country?.Trim(),
            clientType: clientType?.Trim(),
            occurredAt: occurredAt ?? now,
            createdAt: now
        );
    }

    public static DomainValidationResult Validate(string? actorType, string? actorId, string? verb)
    {
        var result = new DomainValidationResult();

        if (string.IsNullOrWhiteSpace(actorType))
        {
            result.Errors.Add(new DomainValidationError("ACTOR_TYPE_REQUIRED", "Actor type is required.", "actorType"));
        }
        else
        {
            var normalizedActorType = actorType.Trim().ToLowerInvariant();
            if (normalizedActorType is not ("user" or "admin" or "system" or "guest"))
            {
                result.Errors.Add(new DomainValidationError("ACTOR_TYPE_INVALID",
                    "Actor type must be 'user', 'admin', 'system', or 'guest'.", "actorType"));
            }
            else if ((normalizedActorType == "user" || normalizedActorType == "admin") && string.IsNullOrWhiteSpace(actorId))
            {
                result.Errors.Add(new DomainValidationError("ACTOR_ID_REQUIRED",
                    $"Actor ID is required when actor type is '{normalizedActorType}'.", "actorId"));
            }
        }

        if (string.IsNullOrWhiteSpace(verb))
        {
            result.Errors.Add(new DomainValidationError("VERB_REQUIRED", "Verb is required.", "verb"));
        }

        return result;
    }
}