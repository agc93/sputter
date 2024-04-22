using Sputter.Core;

namespace Sputter.MQTT.HomeAssistant;

public class TemperatureSensorExtractor : SensorExtractor {
	public override string Name => "Drive Temperature";

	public override string AttributeName => DriveAttributes.Temperature;

	public override string DeviceClass => "temperature";
}
