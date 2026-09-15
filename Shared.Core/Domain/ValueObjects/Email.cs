using System.Net.Mail;
using Shared.Core.Domain.Exceptions;

namespace Shared.Core.Domain.ValueObjects;

public record Email
{
    public Email(string value)
    {
        IllegalDomainException.ThrowIfNull(value, nameof(Email));
        IllegalDomainException.ThrowIfEmpty(value, nameof(Email));
        try
        {
            var address = new MailAddress(value);

            if (!address.Host.Contains('.')) throw new IllegalDomainException($"Email format is not valid: {address}");

            Value = value;
        }
        catch (FormatException)
        {
            throw new IllegalDomainException($"Email format is not valid: {value}");
        }
    }

    public string Value { get; init; }


    public static Email? FromNullable(string? value)
    {
        return value == null ? null : new Email(value);
    }

    public static implicit operator string(Email? email)
    {
        return email is null ? null! : email.Value;
    }

    public static implicit operator Email?(string? value)
    {
        return value == null ? null : new Email(value);
    }
}