namespace Shared.Core.Domain.Exceptions;

public class UnauthorizedAccessException : BaseDomainException
{
    public UnauthorizedAccessException() : base("Unauthorized access.")
    {
    }

    public UnauthorizedAccessException(string message) : base(message)
    {
    }
}
