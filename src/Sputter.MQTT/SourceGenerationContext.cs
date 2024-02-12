using Sputter.MQTT.HomeAssistant;
using System.Text.Json.Serialization;

namespace Sputter.MQTT;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization
)]
[JsonSerializable(typeof(MQTTStatePayload))]
[JsonSerializable(typeof(HomeAssistantDiscoveryPayload))]
internal partial class SourceGenerationContext : JsonSerializerContext {
}
