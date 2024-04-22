using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class MeasurementAggregatorBehaviour : IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> {
	public async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> Handle(DriveMeasurementRequest request, RequestHandlerDelegate<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> next, CancellationToken cancellationToken) {
		var response = await next();
		var merged = MeasurementAggregator.AggregateMeasurements(response);
		return merged;
	}
}
