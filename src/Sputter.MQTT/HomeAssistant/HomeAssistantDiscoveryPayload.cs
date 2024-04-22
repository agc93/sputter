using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Sputter.MQTT.HomeAssistant;

public class HomeAssistantDiscoveryPayload {
	public HomeAssistantDiscoveryPayload(string deviceIdentifier, string deviceName) {
		Device = new HomeAssistantDevicePayload([deviceIdentifier], deviceName);
	}

	[SetsRequiredMembers]
	public HomeAssistantDiscoveryPayload(HomeAssistantDevicePayload device, string stateTopic) {
		Device = device;
		StateTopic = stateTopic;
	}

	public required HomeAssistantDevicePayload Device { get; set; }

	[JsonPropertyName("device_class")]
	public string? DeviceClass { get; set; }

	[JsonPropertyName("enabled_by_default")]
	public bool? EnabledByDefault { get; set; }

	[JsonPropertyName("entity_category")]
	public string? EntityCategory { get; set; }

	[JsonPropertyName("expire_after")]
	public int? ExpiryTimeSeconds { get; set; }

	public string? Name { get; set; }

	[JsonPropertyName("state_class")]
	public string? StateClass { get; set; }

	[JsonPropertyName("state_topic")]
	public required string StateTopic { get; set; }

	[JsonPropertyName("unique_id")]
	public string? UniqueId { get; set; }

	[JsonPropertyName("unit_of_measurement")]
	public string? Unit { get; set; }

	[JsonPropertyName("value_template")]
	public string? ValueTemplate { get; set; }

	[JsonPropertyName("payload_on")]
	public string? OnStatePayload { get; set; }

	[JsonPropertyName("payload_off")]
	public string? OffStatePayload { get; set; }
}
