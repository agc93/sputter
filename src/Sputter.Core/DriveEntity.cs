using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Sputter.Core;

public class DriveEntity {
	[JsonConstructor]
	[Obsolete("Only used for JSON serialization", false)]
	public DriveEntity() { }

	[SetsRequiredMembers]
	public DriveEntity(string id, UniqueId uniqueId) {
		Id = id;
		UniqueId = uniqueId;
		Model = uniqueId.ModelNumber;
	}

	public string? Name { get; set; }
	public required string Id { get; set; }

	public string? Manufacturer { get; set; }
	public string? Model { get; set; }
	public string? SoftwareVersion { get; set; }
	public string? Capacity { get; set; }

	public required UniqueId UniqueId { get; set; }
}