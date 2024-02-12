using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

public class ScrutinyDeviceDetails {
    [JsonPropertyName("CreatedAt")]
    public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("wwn")]
    public required string WWN { get; set; }
    [JsonPropertyName("device_name")]
    public string? DeviceName { get; set; }
    [JsonPropertyName("device_serial_id")]
    public string? SerialId { get; set; }
    [JsonPropertyName("model_name")]
    public string? ModelName { get; set; }
    [JsonPropertyName("serial_number")]
    public required string SerialNumber { get; set; }
    [JsonPropertyName("device_status")]
    public double? DeviceStatus { get; set; }
    public double? Size { get; set; }
    public string? Firmware { get; set; }
}
