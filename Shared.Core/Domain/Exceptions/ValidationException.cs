namespace Shared.Core.Domain.Exceptions;

public class ValidationException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }


    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base(
            message: "One or more validation errors occurred.",
            code: "VALIDATION_FAILED",
            status: 400,
            title: "Validation Error"
        )
    {
        Errors = errors;
    }


    public ValidationException(IEnumerable<DomainValidationError> domainErrors)
        : base(
            message: "Validation of domain rules failed.",
            code: "DOMAIN_VALIDATION_FAILED",
            status: 400,
            title: "Validation Error"
        )
    {
        Errors = domainErrors
            .GroupBy(
                e => string.IsNullOrWhiteSpace(e.Property) ? "general" : e.Property,
                e => e.Code
            )
            .ToDictionary(
                g => g.Key,
                g => g.ToArray()
            );
    }


    public ValidationException(string property, string errorCode)
        : this(new Dictionary<string, string[]>
        {
            [property] = [errorCode]
        })
    {
    }
}