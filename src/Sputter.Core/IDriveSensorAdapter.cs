namespace Sputter.Core;

public interface IDriveSensorAdapter {
	//bool SupportsPrefiltering { get; }
	int Priority { get; }
	string Name { get; }
	Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter);
	Task<DriveMeasurement?> MeasureDrive(DriveEntity drive);
	Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true);
}
