namespace Shared.Core.Domain.Exceptions;

public sealed record DomainValidationError(
    string Code,
    string Message,
    string? Property = null
);