using FluentAssertions;
using Shared.Core.Context;
using Wolverine;
using Wolverine.Persistence.Durability;

namespace Shared.TestKit.Wolverine;

public static class WolverineStoreTestExtensions
{
    public static async Task<Envelope?> FindIncomingEnvelopeAsync<TMessage>(
        this IMessageStore store,
        ExecutionContextData? expectedContext = null)
    {
        var incomings = await store.Admin.AllIncomingAsync();
        var messageFullName = typeof(TMessage).FullName;

        var query = incomings.Where(x => x.MessageType == messageFullName);

        if (expectedContext?.ActorId != null)
        {
            query = query.Where(x =>
                x.TryGetHeader(ExecutionContextHeaders.ActorId, out var actorId) &&
                actorId == expectedContext.ActorId);
        }

        if (expectedContext?.TraceId != null)
        {
            query = query.Where(x =>
                x.TryGetHeader(ExecutionContextHeaders.TraceId, out var traceId) &&
                traceId == expectedContext.TraceId);
        }

        return query.FirstOrDefault();
    }

    public static async Task<Envelope> ShouldHaveIncomingMessageAsync<TMessage>(
        this IMessageStore store,
        ExecutionContextData? expectedContext = null)
    {
        var envelope = await store.FindIncomingEnvelopeAsync<TMessage>(expectedContext);

        envelope.Should().NotBeNull(
            because: $"expected an incoming envelope for message '{typeof(TMessage).Name}'" +
                     (expectedContext?.ActorId != null ? $" and ActorId '{expectedContext.ActorId}'" : string.Empty) +
                     (expectedContext?.TraceId != null ? $" and TraceId '{expectedContext.TraceId}'" : string.Empty) +
                     " to be persisted in outbox");

        if (expectedContext != null)
        {
            envelope!.ShouldContainContextData(expectedContext);
        }

        return envelope!;
    }

    public static async Task ShouldNotHaveIncomingMessageAsync<TMessage>(
        this IMessageStore store,
        ExecutionContextData? expectedContext = null)
    {
        var envelope = await store.FindIncomingEnvelopeAsync<TMessage>(expectedContext);

        envelope.Should().BeNull(
            because: $"message '{typeof(TMessage).Name}' should NOT exist in the incoming envelope store" +
                     (expectedContext?.ActorId != null ? $" for ActorId '{expectedContext.ActorId}'" : string.Empty) +
                     (expectedContext?.TraceId != null ? $" and TraceId '{expectedContext.TraceId}'" : string.Empty));
    }
}