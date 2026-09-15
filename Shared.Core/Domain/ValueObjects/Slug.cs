using Shared.Core.Domain.Exceptions;

namespace Shared.Core.Domain.ValueObjects;

public record Slug
{
    internal Slug(string value)
    {
        IllegalDomainException.ThrowIfRequired(value, nameof(Slug));
        IllegalDomainException.ThrowIfInvalidLength(value, nameof(Slug), 1, 255);
        Value = value;
    }

    public string Value { get; init; }

    public static Slug Create(string text)
    {
        return SlugUtil.ToSlug(text);
    }

    public static implicit operator string(Slug slug)
    {
        return slug?.Value ?? string.Empty;
    }

    public static implicit operator Slug(string value)
    {
        return new Slug(value);
    }
}