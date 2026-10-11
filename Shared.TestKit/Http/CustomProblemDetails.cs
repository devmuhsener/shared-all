using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.TestKit.Http;

public class CustomProblemDetails
{
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public string? Code { get; set; }

    // JSON kök seviyesindeki "errors" alanını doğrudan karşılar
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> Extensions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Hem doğrudan kökten hem de Extensions altından hata listesini çözen yardımcı getter
    /// </summary>
    public Dictionary<string, string[]> GetNormalizedErrors()
    {
        if (Errors is { Count: > 0 })
        {
            return Errors;
        }

        // Eğer kökte değil de Extensions içinde kalmışsa oradan deserialize et
        foreach (var key in new[] { "errors", "Errors" })
        {
            if (Extensions.TryGetValue(key, out var rawErrors))
            {
                if (rawErrors is JsonElement element && element.ValueKind == JsonValueKind.Object)
                {
                    var deserialized = JsonSerializer.Deserialize<Dictionary<string, string[]>>(
                        element.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (deserialized != null)
                    {
                        return deserialized;
                    }
                }

                if (rawErrors is Dictionary<string, string[]> dict)
                {
                    return dict;
                }
            }
        }

        return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
    }
}