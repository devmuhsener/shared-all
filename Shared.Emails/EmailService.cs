using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Shared.Emails;

public class EmailService(
    IEmailQueue emailQueue,
    IEmailSender emailSender) : IEmailService
{
    public async Task EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        await emailQueue.EnqueueAsync(email, cancellationToken);
    }

    public async Task SendEmailAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        await emailSender.SendAsync(email.To, email.Subject, email.Body, ct: cancellationToken);
    }
}