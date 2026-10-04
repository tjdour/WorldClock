using System.Text.Json.Serialization;

namespace WorldClock.Maui.Models;

public class CityData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("timezone")]
    public string TimeZone { get; set; } = string.Empty;
}