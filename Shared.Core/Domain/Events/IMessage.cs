namespace Shared.Core.Domain.Events;

public interface IMessage
{
    Guid Id { get; }
}