using System.Diagnostics;

namespace Shared.Core.Domain.Exceptions;

public class LockedException : AppException
{
    public DateTimeOffset? LockedUntil { get; }
    public int? RetryAfterSeconds { get; }


    public LockedException(string message, DateTimeOffset? lockedUntil = null, string code = "ACCOUNT_LOCKED")
        : base(
            message: message,
            code: code,
            status: 423,
            title: "Account Locked",
            extensions: BuildExtensions(lockedUntil)
        )
    {
        LockedUntil = lockedUntil;
        if (lockedUntil.HasValue)
        {
            var diff = lockedUntil.Value - DateTimeOffset.UtcNow;
            RetryAfterSeconds = diff.TotalSeconds > 0 ? (int)Math.Ceiling(diff.TotalSeconds) : 0;
        }
    }

    public static IDictionary<string, object?>? BuildExtensions(DateTimeOffset? lockedUntil)
    {
        if (!lockedUntil.HasValue) return null;

        var diff = lockedUntil.Value - DateTimeOffset.UtcNow;

        var retryAfter = diff.TotalSeconds > 0 ? (int)Math.Ceiling(diff.TotalSeconds) : 0;

        return new Dictionary<string, object?>
        {
            ["locked_until"] = lockedUntil.Value.ToString("o"),
            ["retry_after_seconds"] = retryAfter,
        };
    }
}