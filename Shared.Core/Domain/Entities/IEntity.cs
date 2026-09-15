namespace Shared.Core.Domain.Entities;

public interface IEntity<out TId>
{
    public TId Id { get; }
}