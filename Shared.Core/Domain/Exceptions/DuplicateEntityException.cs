namespace Shared.Core.Domain.Exceptions;

public class DuplicateEntityException : BaseDomainException
{
    public DuplicateEntityException(string entityName, string key, object value) : base(
        $"Duplicate {entityName} with provided: {key}: {value}")
    {
    }


    public DuplicateEntityException(string message) : base(message)
    {
    }
}