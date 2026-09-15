namespace Shared.Core.Domain.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }


    public EntityNotFoundException(string entityName, string key, object value) : base(
        $"{entityName} not found with provided '{key}': '{value}'")
    {
    }
}