namespace Sputter.Core;

public static class AggregationExtensions {
	public static Dictionary<DriveEntity, DriveMeasurement?> AggregateMeasurements(this IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements) {
		return MeasurementAggregator.AggregateMeasurements(measurements);
	}
}

public class MeasurementAggregator {
	public static Dictionary<DriveEntity, DriveMeasurement?> AggregateMeasurements(IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements) {
		//var dict = new Dictionary<string, KeyValuePair<>>
		var grouped = measurements.GroupBy(k => k.Key.UniqueId.SerialNumber);
		var output = new Dictionary<DriveEntity, DriveMeasurement?>();
		foreach (var group in grouped) {
			//each group is a key (serial number), with a value of:
			// - different drive entities for the same drive
			// - measurement object from each adapter
			// - each measurement will have overlapping sensors and states
			// foreach'ing, the group is effectively foreaching each adapter's results
			var currentAdapterEntity = group.First().Key;
			//var measurement = new DriveMeasurement(group.First().Key.UniqueId.SerialNumber);
			var uniqueKey = group.First().Key.UniqueId;
			// ^^ the first result will be from the highest (i.e. lowest) priority adapter, so trust that
			// vv do we already have an entry for this drive in the output
			var existing = output.FirstOrDefault(o => o.Key.UniqueId == uniqueKey);
			if (existing.Key == null) {
				// we don't have this drive in the output
				var merged = BuildMergedMeasurement(group, uniqueKey, null);
				output.Add(group.First().Key, merged);

			} else {
				output.Remove(existing.Key, out var currentMeasure);
				DriveMeasurement newMeasurement = BuildMergedMeasurement(group, uniqueKey, currentMeasure);
				var key = BuildMergedEntity(existing.Key, currentAdapterEntity);
				output.Add(key, newMeasurement);


				//var newMeasurement = BuildMergedMeasurement(group, uniqueKey, existing.Value);
				//output[existing.Key] = newMeasurement;
			}
			//Console.WriteLine($"{group.Count()} results for '{group.Key}'");

		}
		return output;
	}

	private static DriveMeasurement BuildMergedMeasurement(IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> group, UniqueId uniqueKey, DriveMeasurement? currentMeasure) {
		var newMeasurement = currentMeasure ?? new DriveMeasurement(uniqueKey);
		foreach (var measure in group.Where(g => g.Value != null).SelectMany(g => g.Value!.Sensors)) {
			if (!newMeasurement.Sensors.Any(s => s.AttributeName == measure.AttributeName)) {
				newMeasurement.Sensors.Add(measure);
			}
		}
		foreach (var measure in group.Where(g => g.Value != null).SelectMany(g => g.Value!.States)) {
			if (!newMeasurement.States.Any(s => s.AttributeName == measure.AttributeName)) {
				newMeasurement.States.Add(measure);
			}
		}

		return newMeasurement;
	}

	private static DriveEntity BuildMergedEntity(DriveEntity entity, DriveEntity? target) {
		entity.UniqueId.ModelNumber ??= target?.UniqueId.ModelNumber;
		entity.UniqueId.WWN ??= target?.UniqueId.WWN;
		return entity;
	}
}
