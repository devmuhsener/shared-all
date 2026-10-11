using Bogus;
using Shared.Core.Context;

namespace Shared.TestKit.Context;

public class ExecutionContextBuilder
{
    private static readonly Faker Faker = new();

    private string? _traceId = Guid.NewGuid().ToString("N");
    private string? _tenantId;
    private string? _actorId = Guid.NewGuid().ToString("N");
    private string _actorType = "user";
    private string? _impersonatorId;
    private string _executionType = "interactive";
    private string? _clientIp = Faker.Internet.Ip();
    private string? _userAgent = Faker.Internet.UserAgent();
    private string? _device = "Desktop";
    private string? _country = "TR";
    private string? _clientType = "Web";

    public static ExecutionContextBuilder AnExecutionContextData() => new();

    public ExecutionContextBuilder WithTraceId(string? traceId)
    {
        _traceId = traceId;
        return this;
    }

    public ExecutionContextBuilder WithTenantId(string? tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public ExecutionContextBuilder WithActor(string actorId, string actorType = "user")
    {
        _actorId = actorId;
        _actorType = actorType;
        return this;
    }

    public ExecutionContextBuilder WithActorId(string? actorId)
    {
        _actorId = actorId;
        return this;
    }

    public ExecutionContextBuilder WithActorType(string actorType)
    {
        _actorType = actorType;
        return this;
    }

    public ExecutionContextBuilder WithImpersonatorId(string? impersonatorId)
    {
        _impersonatorId = impersonatorId;
        return this;
    }

    public ExecutionContextBuilder WithExecutionType(string executionType)
    {
        _executionType = executionType;
        return this;
    }

    public ExecutionContextBuilder WithClientIp(string? clientIp)
    {
        _clientIp = clientIp;
        return this;
    }

    public ExecutionContextBuilder WithUserAgent(string? userAgent)
    {
        _userAgent = userAgent;
        return this;
    }

    public ExecutionContextBuilder WithDevice(string? device)
    {
        _device = device;
        return this;
    }

    public ExecutionContextBuilder WithCountry(string? country)
    {
        _country = country;
        return this;
    }

    public ExecutionContextBuilder WithClientType(string? clientType)
    {
        _clientType = clientType;
        return this;
    }

    public ExecutionContextBuilder AsSystem(string? traceId = null, string executionType = "scheduled_job")
    {
        _traceId = traceId;
        _tenantId = null;
        _actorId = null;
        _actorType = "system";
        _impersonatorId = null;
        _executionType = executionType;
        _clientIp = null;
        _userAgent = null;
        _device = null;
        _country = null;
        _clientType = "Worker";
        return this;
    }

    public ExecutionContextData Build()
    {
        return new ExecutionContextData(
            TraceId: _traceId,
            TenantId: _tenantId,
            ActorId: _actorId,
            ActorType: _actorType,
            ImpersonatorId: _impersonatorId,
            ExecutionType: _executionType,
            ClientIp: _clientIp,
            UserAgent: _userAgent,
            Device: _device,
            Country: _country,
            ClientType: _clientType
        );
    }
}
