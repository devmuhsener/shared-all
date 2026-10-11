using Shared.Core.Context;
using Shared.TestKit.Activities;
using Shared.TestKit.Context;
using Shared.TestKit.Hosting;
using Shared.TestKit.MediatR;
using Shared.TestKit.Wolverine;

namespace Shared.TestKit.Base;

public abstract class TestBase(ITestFixture fixture)
{
    protected ITestFixture Fixture = fixture;

    public OutboxTestHelper Outbox { get; } = new(fixture.Services, fixture.Host);

    public ActivityAsserter Activities { get; } = new(fixture.Services);

    public MediatRTestHelper MediatR { get; } = new(fixture.Services);

    public ExecutionContextBuilder ContextBuilder => new();
    public ExecutionContextData RandomContext => ContextBuilder.Build();
    

}