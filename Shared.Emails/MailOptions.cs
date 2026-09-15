namespace Shared.Emails;

public class MailOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public bool UseSsl { get; set; } = false;
    public string SenderEmail { get; set; } = "no-reply@identity.local";
    public string SenderName { get; set; } = "Identity System";
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
