using System.Text.Json.Serialization;

namespace Sputter.Server.Configuration.HomeAssistant;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(HomeAssistantConfigurationSchema))]
internal partial class HomeAssistantSchemaContext : JsonSerializerContext {
}