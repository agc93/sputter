using MediatR;
using Sputter.Core;

namespace Sputter.Messaging;

public class MeasurementRoundingBehaviour : IPipelineBehavior<DriveMeasurementRequest, IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> {
	public async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> Handle(DriveMeasurementRequest request, RequestHandlerDelegate<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> next, CancellationToken cancellationToken) {
		var response = await next();
		foreach (var kvp in response)
		{
			if (kvp.Value != null) {
				kvp.Value.Sensors = kvp.Value.Sensors.Select(ds => {
					ds.Value = Math.Round(ds.Value, 2);
					return ds;
				}).ToList();
			}
		}
		return response;
	}
}
