using System.Text.Json.Serialization;

namespace Sputter.Server.Workers;

public class MQTTWorkerCommand {
    [JsonPropertyName("cmd")]
    public required string Command { get; set; }
}
