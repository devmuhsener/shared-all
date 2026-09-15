using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Emails;

public class EmailBackgroundWorker(IEmailQueue emailQueue, IEmailService emailService, ILogger<EmailBackgroundWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Email Background Worker started listening to email queue.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var email = await emailQueue.DequeueAsync(stoppingToken);
                await emailService.SendEmailAsync(email, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing email from queue.");
            }
        }

        logger.LogInformation("Email Background Worker stopping.");
    }
}
