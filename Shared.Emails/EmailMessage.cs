namespace Shared.Emails;

public record EmailMessage(
    string To,
    string Subject,
    string Body,
    EmailPriority Priority
);

public enum EmailPriority
{
    Critical, // Agresif Retry
    Standard, // Orta seviye retry
    Marketing, // Bir tane dene, olmazsa sal şekli.
}