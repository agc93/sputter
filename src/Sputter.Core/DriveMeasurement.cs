namespace Sputter.Core;

public class DriveMeasurement(UniqueId id) {
    public UniqueId Id { get; } = id;

    public List<DriveSensor> Sensors { get; set; } = [];
    public List<DriveState> States { get; set; } = [];
}
