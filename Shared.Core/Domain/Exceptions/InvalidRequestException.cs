namespace Shared.Core.Domain.Exceptions;

public class InvalidRequestException : BaseDomainException
{
    public InvalidRequestException(string message) : base(message)
    {
    }

    public InvalidRequestException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}