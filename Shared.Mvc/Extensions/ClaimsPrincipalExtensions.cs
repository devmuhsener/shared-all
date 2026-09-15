using System.Security.Claims;
using Shared.Core.Domain.Exceptions;
using Shared.Core.Domain.ValueObjects;

namespace Shared.Mvc.Extensions;

public static class ClaimsPrincipalExtensions
{
    private static readonly string[] UserIdClaimTypes =
    [
        ClaimTypes.NameIdentifier,
        "sub",
        "user_id"
    ];

    public static TUserId GetUserId<TUserId>(this ClaimsPrincipal principal)
        where TUserId : notnull
    {
        if (TryGetUserId(principal, out TUserId userId))
        {
            return userId;
        }

        throw new AuthenticationRequiredException("Current user id claim is missing or invalid.");
    }

    public static bool TryGetUserId<TUserId>(this ClaimsPrincipal principal, out TUserId userId)
        where TUserId : notnull
    {
        userId = default!;

        if (principal is null)
        {
            return false;
        }

        var claimValue = GetUserIdClaimValue(principal);
        if (string.IsNullOrWhiteSpace(claimValue))
        {
            return false;
        }

        return TryConvertUserId(claimValue, out userId);
    }

    private static string? GetUserIdClaimValue(ClaimsPrincipal principal)
    {
        foreach (var claimType in UserIdClaimTypes)
        {
            var value = principal.FindFirstValue(claimType);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool TryConvertUserId<TUserId>(string claimValue, out TUserId userId)
        where TUserId : notnull
    {
        var targetType = typeof(TUserId);

        if (targetType == typeof(Guid))
        {
            if (Guid.TryParse(claimValue, out var guid))
            {
                userId = (TUserId)(object)guid;
                return true;
            }

            userId = default!;
            return false;
        }

        if (typeof(BaseId<Guid>).IsAssignableFrom(targetType))
        {
            if (!Guid.TryParse(claimValue, out var guid))
            {
                userId = default!;
                return false;
            }

            var ctor = targetType.GetConstructor(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic,
                binder: null,
                new[] { typeof(Guid) },
                modifiers: null);

            if (ctor is null)
            {
                userId = default!;
                return false;
            }

            userId = (TUserId)ctor.Invoke([guid]);
            return true;
        }

        userId = default!;
        return false;
    }
}
