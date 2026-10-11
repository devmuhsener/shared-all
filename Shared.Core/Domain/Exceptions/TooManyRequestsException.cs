namespace Shared.Core.Domain.Exceptions;

public class TooManyRequestsException : AppException
{
    public int? RetryAfterSeconds { get; }

    public TooManyRequestsException(
        string message,
        string code = "TOO_MANY_REQUESTS",
        int? retryAfterSeconds = null
    ) : base(
        message: message,
        code: code,
        status: 429,
        title: "Too Many Requests",
        extensions: retryAfterSeconds.HasValue
            ? new Dictionary<string, object?> { { "retryAfterSeconds", retryAfterSeconds.Value.ToString() } }
            : null
    )
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}