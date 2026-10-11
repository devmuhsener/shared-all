using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Emails;

public static class SharedEmailsExtensions
{
    public static IServiceCollection AddSharedEmailsCore(this IServiceCollection services, IConfiguration configuration)
    {
        // ======== OPTIONS ==============
        AddOptions(services, configuration);


        // ========= EMAIL SERVICES ============

        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        services.AddTransient<IEmailSender, MailKitEmailSender>();
        services.AddTransient<IEmailService, EmailService>();


        return services;
    }

    public static IServiceCollection UseInMemoryEmailQueue(this IServiceCollection services)
    {
        services.AddSingleton<IEmailQueue, ChannelEmailQueue>();
        services.AddHostedService<EmailBackgroundWorker>();
        return services;
    }


    private static void AddOptions(IServiceCollection services, IConfiguration configuration)
    {
        // ===== MAIL OPTIONS ========
        var mailOptions = configuration.GetSection(MailOptions.SectionName).Get<MailOptions>() ?? new MailOptions();
        services.AddSingleton(mailOptions);
        services.Configure<MailOptions>(configuration.GetSection(MailOptions.SectionName));
    }
}