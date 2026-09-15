namespace Shared.Core.Domain.Exceptions;

public class PersistenceException : BaseDomainException
{
    public PersistenceException(string message) : base(message)
    {
    }

    public PersistenceException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}