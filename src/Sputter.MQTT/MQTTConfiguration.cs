using Sputter.MQTT.HomeAssistant;

namespace Sputter.MQTT;

public class MQTTConfiguration {
    public required string Server { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }
    public bool? EnableHomeAssistant { get; set; }
    public HomeAssistantConfiguration? HomeAssistant { get; set; }
    public int ReconnectDelay { get; set; } = 5;
}
