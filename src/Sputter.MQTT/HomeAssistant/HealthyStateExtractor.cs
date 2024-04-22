using Sputter.Core;

namespace Sputter.MQTT.HomeAssistant;

public class HealthyStateExtractor : BinarySensorExtractor {
	public override string Name => "Drive Healthy";
	public override string AttributeName => DriveAttributes.Healthy;
	public override string DeviceClass => "problem";
	//override valuetemplate to reverse the bool?
}
