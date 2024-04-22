using Sputter.Core;

namespace Sputter.MQTT.HomeAssistant;

public interface ISensorExtractor {
	string Name { get; }

	bool Supports(DriveMeasurement measurement);

	Task<KeyValuePair<string, HomeAssistantSensorDetails>?> BuildSensor(MeasurementResult result);
}
