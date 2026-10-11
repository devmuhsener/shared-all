namespace Shared.Core.Domain.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(
        string message = "You do not have permission to perform this action.",
        string code = "FORBIDDEN_ACCESS",
        IDictionary<string, object?>? extensions = null
    ) : base(message, code, 403, title: "Forbidden", extensions: extensions)
    {
    }

    public ForbiddenException(
        string message,
        string requiredPermission,
        string code = "FORBIDDEN_ACCESS"
    ) : base(
        message: message,
        code: code,
        status: 403,
        title: "Forbidden",
        extensions: new Dictionary<string, object?>
        {
            ["requiredPermission"] = requiredPermission
        }
    )
    {
    }
}
