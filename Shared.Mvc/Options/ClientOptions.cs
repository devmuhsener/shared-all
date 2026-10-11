namespace Shared.Mvc.Options;

public class ClientOptions
{
    public const string SectionName = "ClientSettings";

    public bool PreferCloudflareHeaders { get; set; } = false;
}