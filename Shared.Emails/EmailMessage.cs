namespace Shared.Emails;

public record EmailMessage(
    string To,
    string Subject,
    string Body
);
