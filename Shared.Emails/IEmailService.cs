namespace Shared.Emails;

public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default);
    ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken = default);
}

public interface IEmailService
{
    Task EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default);
    Task SendEmailAsync(EmailMessage email, CancellationToken cancellationToken = default);
}
