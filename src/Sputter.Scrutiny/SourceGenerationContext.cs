using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization
)]
[JsonSerializable(typeof(ApiResponse<SummaryResponse>))]
internal partial class SourceGenerationContext : JsonSerializerContext {
}
