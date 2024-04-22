using MediatR;
using Sputter.Core;
using System.Text.Json;

namespace Sputter.Messaging;

public class DriveDiscoveryRequest : IRequest<IEnumerable<DriveEntity>> {
	public string? DriveFilter { get; set; }
	public List<DiscoveryTemplate> Templates { get; set; } = [];
	public List<IDriveSensorAdapter> AdditionalAdapters { get; set; } = [];
}
