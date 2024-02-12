using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

public class SummaryResponse {
    [JsonPropertyName("summary")]
    public Dictionary<string, ScrutinyDriveSummary> DriveSummaries { get; set; } = [];
}
