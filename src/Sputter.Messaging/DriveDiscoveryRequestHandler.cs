using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class DriveDiscoveryRequestHandler(IEnumerable<IDriveSensorAdapter> adapters, IEnumerable<IPublishTarget> publishers) : IRequestHandler<DriveDiscoveryRequest, IEnumerable<DriveEntity>> {

    public async Task<IEnumerable<DriveEntity>> Handle(DriveDiscoveryRequest request, CancellationToken cancellationToken) {
        var allAdapters = (adapters ?? []).Concat(request.AdditionalAdapters ?? []);
        var service = new DriveMeasurementService(allAdapters, publishers ?? []);
        var drives = await service.DiscoverDrivesAsync(request.DriveFilter, request.Templates);
        return drives;
    }
}
