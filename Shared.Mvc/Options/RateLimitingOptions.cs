namespace Shared.Mvc.Options;

public class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public List<RateLimitPolicyOptions> Policies { get; set; } = new();
}

public class RateLimitPolicyOptions
{
    public string Name { get; set; } = string.Empty;
    public string PartitionKey { get; set; } = "ClientIp"; // ClientIp | UserId | Global
    public List<RateLimitRuleOptions> Limits { get; set; } = new();
}

public class RateLimitRuleOptions
{
    public int PermitLimit { get; set; }
    public TimeSpan Window { get; set; }
    public int SegmentsPerWindow { get; set; } = 1;
    public int QueueLimit { get; set; } = 0;
}
