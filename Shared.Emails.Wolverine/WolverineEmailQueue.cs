using Shared.Wolverine;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace Shared.Emails.Wolverine;

public class WolverineEmailQueue(IOutboxCoordinator coordinator) : IEmailQueue
{
    public async ValueTask EnqueueAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        object command = message.Priority switch
        {
            EmailPriority.Critical => new SendCriticalEmailCommand(message.To, message.Subject, message.Body),
            EmailPriority.Standard => new SendStandardEmailCommand(message.To, message.Subject, message.Body),
            EmailPriority.Marketing => new SendMarketingEmailCommand(message.To, message.Subject, message.Body),
            _ => throw new ArgumentOutOfRangeException(nameof(message.Priority))
        };

        await coordinator.PublishAsync(command);
    }


    public ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}