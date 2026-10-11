using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IPNetwork = System.Net.IPNetwork;

namespace Shared.Mvc.Extensions;

public static class ForwardedHeadersExtensions
{
    public const string SectionName = "ForwardedHeaders";

    public static IServiceCollection AddSharedForwardedHeaders(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();

            var section = configuration.GetSection(SectionName);

            var forwardLimit = section.GetValue<int?>("ForwardLimit");
            if (forwardLimit.HasValue)
            {
                options.ForwardLimit = forwardLimit.Value;
            }

            var networks = section.GetSection("KnownNetworks").Get<List<string>>() ?? [];
            foreach (var network in networks)
            {
                if (IPNetwork.TryParse(network, out var ipNetwork))
                {
                    options.KnownIPNetworks.Add(ipNetwork);
                }
            }

            var proxies = section.GetSection("KnownProxies").Get<List<string>>() ?? [];
            foreach (var proxy in proxies)
            {
                if (IPAddress.TryParse(proxy, out var ipAddress))
                {
                    options.KnownProxies.Add(ipAddress);
                }
            }
        });

        return services;
    }
    
    public static IApplicationBuilder UseSharedForwardedHeaders(this IApplicationBuilder app)
    {
        return app.UseForwardedHeaders();
    }
}