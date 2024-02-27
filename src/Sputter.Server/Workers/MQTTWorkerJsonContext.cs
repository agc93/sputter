using System.Text.Json.Serialization;

namespace Sputter.Server.Workers;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(MQTTWorkerCommand))]
internal partial class MQTTWorkerJsonContext : JsonSerializerContext {
}