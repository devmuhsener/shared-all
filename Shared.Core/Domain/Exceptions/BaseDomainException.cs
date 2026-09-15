namespace Shared.Core.Domain.Exceptions;

public abstract class BaseDomainException : Exception
{
    public BaseDomainException(string message) : base(message)
    {
    }

    public BaseDomainException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}