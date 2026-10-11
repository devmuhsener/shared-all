using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.Mvc.Options;

namespace Shared.Mvc;

public interface IIpResolver
{
    string? ResolveIp(HttpContext httpContext);
}

public class IpResolver(IOptions<ClientOptions> clientOptions) : IIpResolver
{
    private readonly ClientOptions _options = clientOptions.Value;

    public string? ResolveIp(HttpContext httpContext)
    {
        if (_options.PreferCloudflareHeaders &&
            httpContext.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp) &&
            !string.IsNullOrWhiteSpace(cfIp))
        {
            return cfIp.ToString().Trim();
        }

        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        return !string.IsNullOrWhiteSpace(remoteIp) ? remoteIp : null;
    }
}