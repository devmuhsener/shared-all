using FluentAssertions;
using Shared.Core.Context;
using Shared.Wolverine;
using Wolverine;

namespace Shared.TestKit.Wolverine;

public static class WolverineEnvelopeTestExtensions
{
    public static void ShouldContainContextData(
        this Envelope envelope,
        ExecutionContextData contextData,
        string? executionTypeOverride = "background_job")
    {
        envelope.Should().NotBeNull();

        var expectedOptions = contextData.ToDeliveryOptions(executionTypeOverride);

        foreach (var (expectedKey, expectedVal) in expectedOptions.Headers)
        {
            var keyExists = envelope.Headers.TryGetValue(expectedKey, out var actualVal);
            keyExists.Should().BeTrue($"Expected envelope header '{expectedKey}' to exist.");
            actualVal.Should().Be(expectedVal, $"Expected envelope header '{expectedKey}' to be '{expectedVal}', but found '{actualVal}'.");
        }
    }
}
