﻿namespace Sputter.Core;

public interface IMeasurementService {
	Task<IEnumerable<DriveEntity>> DiscoverDrivesAsync(string? driveSpec, IEnumerable<DiscoveryTemplate>? discoverySpecs);
	Task<List<KeyValuePair<DriveEntity, DriveMeasurement?>>> MeasureDrivesAsync(IEnumerable<DriveEntity> drives);
	IAsyncEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> MeasureDrives(IEnumerable<DriveEntity> drives);
}