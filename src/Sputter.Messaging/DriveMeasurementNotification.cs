using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class DriveMeasurementNotification(IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements) : INotification {
	public IEnumerable<IPublishTarget>? Targets { get; set; }
	public List<KeyValuePair<DriveEntity, DriveMeasurement?>> Measurements { get; } = measurements.ToList();
}
