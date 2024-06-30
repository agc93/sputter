using System.Text.Json.Serialization;

namespace Sputter.Synology;

public class SynologyDiskInfo {
	public required string Device { get; set; }
	public required string Id { get; set; }
	[JsonPropertyName("longName")]
	public required string Name { get; set; }
	[JsonPropertyName("model")]
	public required string Model { get; set; }
	[JsonPropertyName("serial")]
	public required string Serial { get; set; }
	[JsonPropertyName("size_total")]
	public required long SizeBytes { get; set; }
	[JsonPropertyName("temp")]
	public required int Temperature { get; set; }
	[JsonPropertyName("vendor")]
	public required string Vendor { get; set; }
	
	[JsonPropertyName("firm")]
	public required string Firmware { get; set; }
	
	[JsonPropertyName("status")]
	public required string Status { get; set; }
}