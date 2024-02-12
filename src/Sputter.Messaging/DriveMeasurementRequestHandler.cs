using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class DriveMeasurementRequestHandler(IEnumerable<IDriveSensorAdapter> adapters, IEnumerable<IPublishTarget> publishers) : IRequestHandler<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> {
    public async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> Handle(DriveMeasurementRequest request, CancellationToken cancellationToken) {
        var allAdapters = (adapters ?? []).Concat(request.AdditionalAdapters ?? []);
        var service = new DriveMeasurementService(allAdapters, publishers ?? []);
        if (request.Drives == null) {
            var drives = await service.DiscoverDrivesAsync(request.DriveFilter);
            request.Drives = drives;
        }
        request.Drives ??= [];
        var results = await service.MeasureDrives(request.Drives).WaitForAll();
        return results;
    }
}
