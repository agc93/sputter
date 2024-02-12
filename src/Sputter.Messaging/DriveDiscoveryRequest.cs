using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class DriveDiscoveryRequest : IRequest<IEnumerable<DriveEntity>> {
    public string? DriveFilter { get; set; }
    public List<IDriveSensorAdapter> AdditionalAdapters { get; set; } = [];
}
