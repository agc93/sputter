using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

public class ScrutinySmartInfo {
    [JsonPropertyName("collector_date")]
    public DateTimeOffset CollectorDate { get; set; }

    [JsonPropertyName("temp")]
    public double Temperature { get; set; }
    [JsonPropertyName("power_on_hours")]
    public double PowerOnHours { get; set; }
}
