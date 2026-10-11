namespace Shared.Core.Domain.Events;

public interface IEvent : IMessage
{
    public DateTimeOffset OccurredAt { get; }
}