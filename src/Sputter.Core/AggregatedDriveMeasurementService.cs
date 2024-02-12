namespace Sputter.Core;

[Obsolete(DeprecationMessage, false)]
public class AggregatedDriveMeasurementService(IEnumerable<IDriveSensorAdapter> adapters, IEnumerable<IPublishTarget> publishers) : IMeasurementService {
    private const string DeprecationMessage = "Not recommended for use. We recommend using an existing IMeasurementService, and aggregating the results in consuming code";
    private readonly DriveMeasurementService _measurer = new(adapters, publishers);

    public Task<IEnumerable<DriveEntity>> DiscoverDrivesAsync(string? driveSpec) {
        return _measurer.DiscoverDrivesAsync(driveSpec);
    }

    public async IAsyncEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> MeasureDrives(IEnumerable<DriveEntity> drives) {
        var measurements = await _measurer.MeasureDrives(drives).WaitForAll();
        var merged = measurements.AggregateMeasurements();
        foreach (var merge in merged) {
            yield return merge;
        }
    }

    public async Task<List<KeyValuePair<DriveEntity, DriveMeasurement?>>> MeasureDrivesAsync(IEnumerable<DriveEntity> drives) {
        var results = await _measurer.MeasureDrivesAsync(drives);
        var merged = results.AggregateMeasurements();
        return [.. merged];
    }
}
