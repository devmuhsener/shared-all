namespace Shared.Core.Domain.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(
        string message,
        string code,
        IDictionary<string, object?>? extensions = null
    ) : base(message, code, 409, title: "Conflict", extensions: extensions)
    {
    }
}