using Sputter.Messaging;
using Sputter.MQTT;
using Sputter.Server.Configuration;
using System.Text.Json.Serialization;

namespace Sputter.Server;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Serialization
)]
[JsonSerializable(typeof(DriveDiscoveryRequest))]
[JsonSerializable(typeof(DriveMeasurementRequest))]
[JsonSerializable(typeof(ServerConfiguration))]
[JsonSerializable(typeof(MQTTConfiguration))]
internal partial class ChecksumJsonContext : JsonSerializerContext {
}