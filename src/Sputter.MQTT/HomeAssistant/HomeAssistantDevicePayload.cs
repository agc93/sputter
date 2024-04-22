using System.Text.Json.Serialization;

namespace Sputter.MQTT.HomeAssistant;

public class HomeAssistantDevicePayload(IEnumerable<string> idents, string name) {
	public IEnumerable<string> Identifiers { get; set; } = idents;
	public string? Manufacturer { get; set; }
	public string? Model { get; set; }
	public string Name { get; set; } = name;
	[JsonPropertyName("via_device")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
	public string SourceName => "sputter";
	[JsonPropertyName("suggested_area")]
	public string? SuggestedArea { get; set; }
	[JsonPropertyName("sw_version")]
	public string? SoftwareVersion { get; set; }
	[JsonPropertyName("hw_version")]
	public string? HardwareVersion { get; set; }
}
