using FluentResults;

namespace Sputter.Core;

public class DriveMeasurementService(IEnumerable<IDriveSensorAdapter> adapters, IEnumerable<IPublishTarget> publishTargets) : IMeasurementService {
	private readonly List<IDriveSensorAdapter> _adapters = adapters.ToList();
	private readonly List<IPublishTarget> _publishTargets = publishTargets.ToList();
	public bool Debug { get; set; } = false;

	[Obsolete("Prefer providing discovery templates")]
	public async Task<IEnumerable<DriveEntity>> DiscoverDrivesAsync(string? driveSpec) {
		var drives = new List<DriveEntity>();
		foreach (var ad in _adapters.OrderBy(a => a.Priority)) {
			drives.AddRange(await ad.DiscoverDrives(driveSpec));
		}
		return drives;
	}

	public async Task<IEnumerable<DriveEntity>> DiscoverDrivesAsync(string? driveSpec, IEnumerable<DiscoveryTemplate>? discoverySpecs) {
		discoverySpecs ??= [];
		if (!string.IsNullOrWhiteSpace(driveSpec)) {
			discoverySpecs = discoverySpecs.Append(new DiscoveryTemplate(driveSpec));
		}
		var drives = new List<DriveEntity>();
		foreach (var ad in _adapters.OrderBy(a => a.Priority)) {
			if (discoverySpecs.Any()) {
				foreach (var spec in discoverySpecs.Where(ds => MatchesAdapter(ds, ad))) {
					drives.AddRange(await ad.DiscoverDrives(spec.Match));
				}
			} else {
				//this should only ever be null, since if it wasn't, it should have been transformed into a discovery spec
				drives.AddRange(await ad.DiscoverDrives(driveSpec));
			}
		}
		return drives;
	}

	private static bool MatchesAdapter(DiscoveryTemplate template, IDriveSensorAdapter adapter) {
		return template.SourceAdapter == null
			|| template.SourceAdapter == "*"
			|| string.Equals(template.SourceAdapter, adapter.Name, StringComparison.InvariantCultureIgnoreCase);
	}

	public async Task<List<KeyValuePair<DriveEntity, DriveMeasurement?>>> MeasureDrivesAsync(IEnumerable<DriveEntity> drives) {
		var res = MeasureDrives(drives);
		return await res.WaitForAll();
	}

	//public async IAsyncEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> StreamDriveMeasurementsAsync(string? driveSpec) {
	//    var drives = new List<DriveEntity>();
	//    foreach (var ad in _adapters) {
	//        drives.AddRange(await ad.DiscoverDrives(driveSpec));
	//    }
	//    Console.WriteLine(drives.Count);
	//    List<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements = [];
	//    //var drives = adapters.SelectMany(ad => ad.DiscoverDrives(null));
	//    foreach (var drive in drives) {
	//        DriveMeasurement? measure = null;
	//        for (var i = 0; measure == null && i < _adapters.Count; i++) {
	//            measure = await _adapters[i].MeasureDrive(drive);
	//        }
	//        if (measure != null) {
	//            Console.WriteLine($"Drive: {drive.Id}");
	//            Console.WriteLine($"Unique ID: {drive.UniqueId}");
	//            Console.WriteLine(string.Join(Environment.NewLine, measure.Sensors.Select(s => s.ToString())));
	//            Console.WriteLine(string.Join(Environment.NewLine, measure.States.Select(s => $"{s.AttributeName}: {s.Value}")));
	//            Console.WriteLine();
	//            yield return new KeyValuePair<DriveEntity, DriveMeasurement?>(drive, measure);
	//        }
	//    }
	//}

	public async Task<Dictionary<UniqueId, IEnumerable<Result>>> PublishMeasurementsAsync(IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements) {
		var results = PublishMeasurementsResultsAsync(measurements);
		var output = new Dictionary<UniqueId, IEnumerable<Result>>();
		await foreach (var result in results) {
			output.Add(result.Key, result.Value);
		}
		return output;
	}

	public async IAsyncEnumerable<KeyValuePair<UniqueId, IEnumerable<Result>>> PublishMeasurementsResultsAsync(IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements) {
		void Log(string message) {
			if (Debug) {
				Console.WriteLine(message);
			}
		}
		foreach (var uniqueDrive in measurements) {
			var currentResults = new List<Result>();
			if (uniqueDrive.Value != null) {
				foreach (var publisher in _publishTargets) {
					try {
						var res = await publisher.PublishMeasurement(new MeasurementResult(uniqueDrive.Key, uniqueDrive.Value!));
						if (res.IsSuccess) {
							Log($"Succesfully published '{uniqueDrive.Key.UniqueId}' to '{publisher.GetType().Name}'");
						} else {
							Log($"Failed to publish measurement for '{uniqueDrive.Key.UniqueId}' to '{publisher.GetType().Name}'");
						}
						currentResults.Add(res);
					} catch (NotImplementedException) {
						//ignored
					} catch {
						Console.WriteLine($"Error encountered while publishing to '{publisher.GetType().Name}'");
					}
				}
			}
			yield return new KeyValuePair<UniqueId, IEnumerable<Result>>(uniqueDrive.Key.UniqueId, currentResults);
		}
	}

	public async IAsyncEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> MeasureDrives(IEnumerable<DriveEntity> drives) {
		foreach (var drive in drives) {
			DriveMeasurement? measure = null;
			for (var i = 0; measure == null && i < _adapters.Count; i++) {
				measure = await _adapters[i].MeasureDrive(drive);
				measure.AddSource(_adapters[i].Name);
			}
			if (measure != null) {
				yield return new KeyValuePair<DriveEntity, DriveMeasurement?>(drive, measure);
			}
		}
	}
}
