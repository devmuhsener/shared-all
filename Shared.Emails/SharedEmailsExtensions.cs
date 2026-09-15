using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Emails;

public static class SharedEmailsExtensions
{
    public static IServiceCollection AddEmailsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // ======== OPTIONS ==============
        AddOptions(services, configuration);

        
        // ========= EMAIL SERVICES ============
        services.AddSingleton<IEmailQueue, EmailQueue>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        
        
        // ========== HOSTED SERVICES ================
        services.AddHostedService<EmailBackgroundWorker>();
        return services;
    }


    private static void AddOptions(IServiceCollection services, IConfiguration configuration)
    {
        // ===== MAIL OPTIONS ========
        var mailOptions =
            configuration.GetSection("Mail").Get<MailOptions>() ?? new MailOptions();
        services.AddSingleton(mailOptions);
    }
}