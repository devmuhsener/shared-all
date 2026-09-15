namespace Shared.Core.Domain.Exceptions;

public abstract class NotFoundException(string message, Exception? innerException = null) : Exception(message, innerException);