using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

public class ScrutinyDriveSummary {
    [JsonPropertyName("device")]
    public required ScrutinyDeviceDetails Device { get; set; }
    [JsonPropertyName("smart")]
    public required ScrutinySmartInfo SmartInfo { get; set; }
}
