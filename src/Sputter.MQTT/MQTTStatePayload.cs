using System.Text.Json.Serialization;

namespace Sputter.MQTT;

public class MQTTStatePayload {
    [JsonPropertyName("sensors")]
    public Dictionary<string, double> Sensors { get; set; } = [];
    [JsonPropertyName("states")]
    public Dictionary<string, string> States { get; set; } = [];
}
