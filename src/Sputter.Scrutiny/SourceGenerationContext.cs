using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(ApiResponse<SummaryResponse>))]
[JsonSerializable(typeof(ScrutinyDriveSummary))]
[JsonSerializable(typeof(ScrutinyDeviceDetails))]
[JsonSerializable(typeof(ScrutinySmartInfo))]
internal partial class SourceGenerationContext : JsonSerializerContext {
}
