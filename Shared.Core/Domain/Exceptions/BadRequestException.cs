namespace Shared.Core.Domain.Exceptions;

public class BadRequestException : AppException


{
    public BadRequestException(
        string message,
        string code) : base(
        message,
        code: code,
        status: 400,
        title: "Bad Request",
        extensions: new Dictionary<string, object?>()
    )
    {
    }
}