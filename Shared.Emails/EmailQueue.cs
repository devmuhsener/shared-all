using System.Threading.Channels;

namespace Shared.Emails;

public class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _channel = Channel.CreateUnbounded<EmailMessage>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    public ValueTask EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        return _channel.Writer.WriteAsync(email, cancellationToken);
    }

    public ValueTask<EmailMessage> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}
