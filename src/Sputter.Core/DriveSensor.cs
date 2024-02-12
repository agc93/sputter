namespace Sputter.Core;

public class DriveSensor : SensorValue<double> {
    public string GetName() {
        return FriendlyName ?? AttributeName;
    }

    public string? Units { get; set; }

    public override string ToString() {
        return $"{GetName()}: {Value:0.00} {Units ?? string.Empty}";
    }
}
