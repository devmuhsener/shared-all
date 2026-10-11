using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Mvc.Handlers;
using Shared.Mvc.Options;

namespace Shared.Mvc.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedMvcModule(this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        services.AddSharedForwardedHeaders(configuration!);
        services.AddSharedErrorHandling();


        // Add Configuration
        if (configuration != null)
        {
            services.Configure<ClientOptions>(configuration.GetSection(ClientOptions.SectionName));
        }

        services.AddScoped<IIpResolver, IpResolver>();


        return services;
    }


    public static IMvcBuilder ConfigureGlobalJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        });
    }


    /// <summary>
    /// ProblemDetails ve GlobalExceptionHandler altyapısını DI'a kaydeder.
    /// </summary>
    private static IServiceCollection AddSharedErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    /// <summary>
    /// Exception handling middleware'ini pipeline'a ekler.
    /// </summary>
    public static IApplicationBuilder UseSharedErrorHandling(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler();
    }
}