namespace Shared.Core.Domain.ValueObjects;

public record BaseId<TKey> where TKey : new()
{
    protected BaseId()
    {
    }

    public BaseId(TKey value)
    {
        Value = value;
    }

    public TKey Value { get; } = default!;

    public static implicit operator TKey?(BaseId<TKey> id)
    {
        return id.Value;
    }

    public static implicit operator BaseId<TKey>(TKey value)
    {
        return new BaseId<TKey>(value);
    }
}