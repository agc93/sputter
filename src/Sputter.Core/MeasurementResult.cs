namespace Sputter.Core;

public class MeasurementResult(DriveEntity entity, DriveMeasurement measurement) {
    public DriveEntity Drive { get; set; } = entity;
    public DriveMeasurement Measurement { get; set; } = measurement;
}
