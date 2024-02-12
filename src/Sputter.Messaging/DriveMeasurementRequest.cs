using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class DriveMeasurementRequest : IRequest<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> {
    public IEnumerable<DriveEntity>? Drives { get; internal set; }
    public string? DriveFilter { get; set; }

    public List<IDriveSensorAdapter> AdditionalAdapters { get; set; } = [];

    public DriveMeasurementRequest(IEnumerable<DriveEntity> drives) {
        Drives = drives;
    }

    public DriveMeasurementRequest(string? driveFilter) {
        DriveFilter = driveFilter;
    }
}
