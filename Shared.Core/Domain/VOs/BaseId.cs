namespace Shared.Core.Domain.VOs;

public record BaseId<TKey> where TKey : notnull
{
    protected BaseId()
    {
    }

    public BaseId(TKey value)
    {
        Value = value;
    }

    public TKey Value { get; } = default!;


    public override string ToString()
    {
        return Value?.ToString() ?? "";
    }


    public static implicit operator TKey?(BaseId<TKey> id)
    {
        return id.Value;
    }

    public static implicit operator BaseId<TKey>(TKey value)
    {
        return new BaseId<TKey>(value);
    }
    
}