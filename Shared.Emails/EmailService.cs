
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Shared.Emails;

public class EmailService(IEmailQueue emailQueue, MailOptions mailOptions, ILogger<EmailService> logger) : IEmailService
{
    public async Task EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Enqueuing email to {To} with subject '{Subject}'", email.To, email.Subject);
        await emailQueue.EnqueueAsync(email, cancellationToken);
    }

    public async Task SendEmailAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Sending email to {To} via SMTP host {Host}:{Port}", email.To, mailOptions.Host, mailOptions.Port);

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mailOptions.SenderName, mailOptions.SenderEmail));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = email.Body
            };

            using var client = new SmtpClient();

            var socketOptions = mailOptions.UseSsl
                ? (mailOptions.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                : SecureSocketOptions.None;

            await client.ConnectAsync(mailOptions.Host, mailOptions.Port, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(mailOptions.UserName) && !string.IsNullOrWhiteSpace(mailOptions.Password))
            {
                await client.AuthenticateAsync(mailOptions.UserName, mailOptions.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email successfully sent to {To}", email.To);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", email.To);
            throw;
        }
    }
}
