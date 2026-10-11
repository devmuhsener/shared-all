using System.Text.RegularExpressions;

namespace Shared.Core.Domain.Exceptions;

public sealed record DomainValidationResult
{
    public List<DomainValidationError> Errors { get; } = [];
    public bool IsValid => Errors.Count == 0;


    public static DomainValidationResult Success => new();

    public static DomainValidationResult Failure(params DomainValidationError[] errors)
    {
        var result = new DomainValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }

    public DomainValidationResult Merge(DomainValidationResult other)
    {
        if (other != null && other.Errors.Count > 0)
        {
            Errors.AddRange(other.Errors);
        }

        return this;
    }

    public void AddError(string code, string message, string property)
    {
        Errors.Add(CreateError(code, message, property));
    }

    public void EnsureValid()
    {
        if (!IsValid)
        {
            throw new ValidationException(Errors);
        }
    }


    // =====================================================
    // HELPER METHODS TO CHECK AND ADD ERROR
    // ======================================================

    public void AddIfNullOrWhiteSpace(string? value, string code, string message, string property)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddError(code, message, property);
        }
    }

    public void AddIfLessThan(int actual, int threshold, string code, string message, string property)
    {
        if (actual < threshold)
        {
            AddError(code, message, property);
        }
    }

    public void AddIfGreaterThan(int actual, int threshold, string code, string message, string property)
    {
        if (actual > threshold)
        {
            AddError(code, message, property);
        }
    }

    public void AddIfDoesNotMatch(string value, Regex regex, string code, string message, string property)
    {
        if (!regex.IsMatch(value))
        {
            AddError(code, message, property);
        }
    }

    public void AddIfContainsAny(List<string> reservedUsernames, string value, string code,
        string message, string property)
    {
        if (reservedUsernames.Any(r => string.Equals(r, value, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(code, message, property);
        }
    }


    public void AddIfTrue(string code, string message, string property, Func<bool> func)
    {
        if (func())
        {
            AddError(code, message, property);
        }
    }

    public void AddIfFalse(string code, string message, string property, Func<bool> func)
    {
        if (!func())
        {
            AddError(code, message, property);
        }
    }


    // ================================================
    // PRIVATE HELPER METHODS 
    // ================================================


    private static DomainValidationError CreateError(string code, string message, string property)
    {
        return new DomainValidationError(code, message, property);
    }
}