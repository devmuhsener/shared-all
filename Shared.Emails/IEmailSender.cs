using System.Runtime.InteropServices.ComTypes;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Shared.Emails;

public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        string? textBody = null,
        CancellationToken ct = default
    );

    Task SendAsync(
        EmailMessage message,
        CancellationToken ct = default
    );
}

public sealed class MailKitEmailSender(IOptions<MailOptions> options, ILogger<MailKitEmailSender> logger) : IEmailSender
{
    private readonly MailOptions _options = options.Value;

    public async Task SendAsync(string to, string subject, string htmlBody, string? textBody = null,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(to, nameof(to));
        ArgumentNullException.ThrowIfNull(subject, nameof(subject));
        ArgumentNullException.ThrowIfNull(htmlBody, nameof(htmlBody));


        var message = CreateMimeMessage(to, subject, htmlBody, textBody);

        using var client = new SmtpClient();
        client.Timeout = _options.TimeoutMilliSeconds;

        try
        {
            var socketOptions = _options.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : (_options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            logger.LogDebug("Connection to SMTP server at {Host}:{Port} using {SocketOptions}", _options.Host,
                _options.Port, socketOptions);

            await client.ConnectAsync(_options.Host, _options.Port, socketOptions, ct);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password, ct);
            }

            logger.LogInformation("Sending email to {Recipient} with subject '{Subject}'", to, subject);

            await client.SendAsync(message, ct);

            logger.LogInformation("Email successfully sent to {Recipient}", to);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send email to {Recipient}. Host: {Host}, Port: {Port}",
                to, _options.Host, _options.Port);
            throw;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, CancellationToken.None);
            }
        }
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        await SendAsync(message.To, message.Subject, message.Body, ct: ct);
    }


    private MimeMessage CreateMimeMessage(string to, string subject, string htmlBody, string? textBody = null)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder()
        {
            TextBody = textBody,
            HtmlBody = htmlBody,
        };

        message.Body = builder.ToMessageBody();
        return message;
    }
}