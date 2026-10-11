namespace Shared.Core.Domain.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(
        string message,
        string code = "NOT_FOUND",
        IDictionary<string, object?>? extensions = null
    ) : base(message, code, 404, title: "Resource Not Found", extensions: extensions)
    {
    }

    public NotFoundException(
        string entityName,
        string keyName,
        object keyValue,
        string code = "RESOURCE_NOT_FOUND"
    ) : base(
        message: $"{entityName} was not found with '{keyName}': '{keyValue}'.",
        code: code,
        status: 404,
        title: "Resource Not Found",
        extensions: new Dictionary<string, object?>
        {
            ["entityName"] = entityName,
            ["keyName"] = keyName,
            ["keyValue"] = keyValue
        }
    )
    {
    }
}
