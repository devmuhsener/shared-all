using System.Net.Mail;
using Shared.Core.Domain.Exceptions;

namespace Shared.Core.Domain.VOs;

public record Email
{
    public Email(string value)
    {
        var result = Validate(value);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        Value = value;
    }

    public string Value { get; init; }


    public static DomainValidationResult Validate(string? email)
    {
        var result = new DomainValidationResult();
        if (string.IsNullOrWhiteSpace(email))
        {
            result.Errors.Add(new DomainValidationError("EMAIL_REQUIRED", "Email is required", "email"));
            return result;
        }

        try
        {
            var address = new MailAddress(email);

            if (!address.Host.Contains('.'))
            {
                result.Errors.Add(new DomainValidationError("EMAIL_INVALID_FORMAT", "Email is invalid.", "email"));
            }
        }
        catch (FormatException e)
        {
            result.Errors.Add(new DomainValidationError("EMAIL_INVALID_FORMAT", "Email is invalid", "email"));
        }

        return result;
    }
}