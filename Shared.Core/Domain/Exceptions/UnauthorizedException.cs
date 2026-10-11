namespace Shared.Core.Domain.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(
        string? message = null,
        string? code = null,
        IDictionary<string, object?>? extensions = null
    ) : base(
        message ?? "Authentication required.",
        code ?? "UNAUTHORIZED",
        status: 401,
        title: "Unauthorized",
        extensions = extensions
    )
    {
    }
}