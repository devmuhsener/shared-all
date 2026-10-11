using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Shared.Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Shared.Emails.Wolverine;

public class WolverineEmailHandler(IEmailSender emailSender, ILogger<WolverineEmailHandler> logger)
    : IMyWolverineHandler
{
    public static void Configure(HandlerChain chain)
    {
        if (chain.MessageType == typeof(SendCriticalEmailCommand))
        {
            chain.OnException<Exception>()
                .RetryWithCooldown(1.Seconds(), 3.Seconds(), 10.Seconds())
                .Then.ScheduleRetry(30.Minutes());
        }
        else if (chain.MessageType == typeof(SendStandardEmailCommand))
        {
            chain.OnException<Exception>()
                .RetryWithCooldown(10.Seconds(), 1.Minutes());
        }
    }


    public async Task HandleAsync(SendCriticalEmailCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("[Sending Critical Mail] To: {To}, Subject : {Subject}", command.To, command.Subject);


        await emailSender.SendAsync(command.To, command.Subject, command.Body, ct: ct);
    }


    public async Task HandleAsync(SendStandardEmailCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("[Sending Standard Mail] To: {To}, Subject : {Subject}", command.To, command.Subject);
        await emailSender.SendAsync(command.To, command.Subject, command.Body, ct: ct);
    }


    public async Task HandleAsync(SendMarketingEmailCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("[Sending Marketing Mail] To: {To}, Subject : {Subject}", command.To, command.Subject);
        await emailSender.SendAsync(command.To, command.Subject, command.Body, ct: ct);
    }
}

public record SendCriticalEmailCommand(string To, string Subject, string Body);

public record SendStandardEmailCommand(string To, string Subject, string Body);

public record SendMarketingEmailCommand(string To, string Subject, string Body);