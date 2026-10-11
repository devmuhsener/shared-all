using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Core.Context;
using Shared.Mvc.Options;

namespace Shared.Mvc.Middlewares;

public class ExecutionContextMiddleware(RequestDelegate next, ILogger<ExecutionContextMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext httpContext,
        IExecutionContextAccessor contextAccessor,
        IOptions<ClientOptions> options)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

        var user = httpContext.User;
        var isAuthenticated = user.Identity?.IsAuthenticated == true;

        var userId = isAuthenticated
            ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value
            : null;


        // If not authenticated -> then "guest".
        // If authenticated and is in role ADMIN -> "Admin";
        // otherwise -> "User"
        var actorType = isAuthenticated
            ? user.IsInRole("ADMIN") || user.IsInRole("Admin") ? "Admin" : "User"
            : "guest";

        var impersonatorId = user.FindFirst("impersonator_id")?.Value;


        var ip = GetIpAddress(httpContext, options.Value);
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();


        var contextData = new ExecutionContextData(
            TraceId: traceId,
            TenantId: user.FindFirst("tenant_id")?.Value,
            ActorId: userId,
            ActorType: actorType,
            ImpersonatorId: impersonatorId,
            ExecutionType: impersonatorId != null ? "impersonated" : "interactive",
            ClientIp: ip,
            UserAgent: userAgent,
            Device: ResolveDevice(httpContext, userAgent),
            Country: GetCountryCode(httpContext, options.Value),
            ClientType: ResolveClientType(httpContext, userAgent)
        );

        contextAccessor.SetContext(contextData);
        logger.LogInformation("ExecutionContext is set to accessor. UserId: {ActorId}, IP: {IP}", userId, ip);

        await next(httpContext);
    }


    private static string? GetCountryCode(HttpContext httpContext, ClientOptions options)
    {
        if (options.PreferCloudflareHeaders &&
            httpContext.Request.Headers.TryGetValue("CF-IPCountry", out var country) &&
            !string.IsNullOrWhiteSpace(country))
        {
            var code = country.ToString().Trim().ToUpperInvariant();
            if (code is not "XX" and not "T1")
            {
                return code;
            }
        }

        return null;
    }

    private static string? ResolveClientType(HttpContext context, string? userAgent)
    {
        // 1. Check explicit X-Client-Type header
        if (context.Request.Headers.TryGetValue("X-Client-Type", out var clientTypeHeader) &&
            !string.IsNullOrWhiteSpace(clientTypeHeader))
        {
            return clientTypeHeader.ToString().Trim();
        }

        // 2. Fallback heuristics based on User-Agent
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            var lowerUa = userAgent.ToLowerInvariant();
            if (lowerUa.Contains("iphone") || lowerUa.Contains("ipad") || lowerUa.Contains("android") ||
                lowerUa.Contains("mobile"))
            {
                return "Mobile";
            }

            return "Web";
        }

        return null;
    }


    private static string? GetIpAddress(HttpContext httpContext, ClientOptions options)
    {
        if (options.PreferCloudflareHeaders &&
            httpContext.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp) &&
            !string.IsNullOrWhiteSpace(cfIp))
        {
            return cfIp.ToString().Trim();
        }

        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        return !string.IsNullOrWhiteSpace(remoteIp) ? remoteIp : null;
    }

    private static string ResolveDevice(HttpContext context, string? userAgent)
    {
        // 1. Check custom mobile app device headers
        if (context.Request.Headers.TryGetValue("X-Device-Model", out var modelHeader) &&
            !string.IsNullOrWhiteSpace(modelHeader))
        {
            return modelHeader.ToString().Trim();
        }

        if (context.Request.Headers.TryGetValue("X-Device-Name", out var nameHeader) &&
            !string.IsNullOrWhiteSpace(nameHeader))
        {
            return nameHeader.ToString().Trim();
        }

        if (context.Request.Headers.TryGetValue("X-Device", out var deviceHeader) &&
            !string.IsNullOrWhiteSpace(deviceHeader))
        {
            return deviceHeader.ToString().Trim();
        }

        // 2. Check Client Hints headers (Sec-CH-UA-Platform)
        if (context.Request.Headers.TryGetValue("Sec-CH-UA-Platform", out var platformHeader) &&
            !string.IsNullOrWhiteSpace(platformHeader))
        {
            var platform = platformHeader.ToString().Replace("\"", "").Trim();
            if (!string.IsNullOrWhiteSpace(platform))
            {
                return platform;
            }
        }

        // 3. User-Agent string parsing fallback
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            var lowerUa = userAgent.ToLowerInvariant();

            if (lowerUa.Contains("iphone")) return "iPhone";
            if (lowerUa.Contains("ipad")) return "iPad";
            if (lowerUa.Contains("macintosh") || lowerUa.Contains("mac os")) return "Mac";
            if (lowerUa.Contains("windows")) return "Windows";
            if (lowerUa.Contains("android")) return "Android";
            if (lowerUa.Contains("linux")) return "Linux";
        }

        return "Unknown";
    }
}