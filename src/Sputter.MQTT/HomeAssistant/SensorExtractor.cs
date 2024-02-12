using Sputter.Core;
using HASensor = System.Collections.Generic.KeyValuePair<string, Sputter.MQTT.HomeAssistant.HomeAssistantSensorDetails>?;

namespace Sputter.MQTT.HomeAssistant;

public abstract class SensorExtractor : ISensorExtractor {
    public abstract string Name { get; }
    public abstract string AttributeName { get; }
    public abstract string DeviceClass { get; }

    public virtual bool Supports(DriveMeasurement measurement) {
        return measurement.Sensors.Any(s => s.AttributeName == AttributeName) || measurement.States.Any(s => s.AttributeName == AttributeName);
    }

    public virtual Task<HASensor> BuildSensor(MeasurementResult result) {
        if (result.Measurement.Sensors.FirstOrDefault(s => s.AttributeName == AttributeName) is var sensor && sensor != null) {
            var sensorTopic = $"homeassistant/sensor/{result.Drive.UniqueId.SerialNumber}/{Name.ToObjectId()}/config";
            return Task.FromResult<HASensor>(new KeyValuePair<string, HomeAssistantSensorDetails>(sensorTopic, new HomeAssistantSensorDetails(result.Drive.UniqueId.GetNamedObjectId(Name)) {
                DeviceClass = DeviceClass,
                FriendlyName = Name,
                Units = sensor.Units,
                ValueTemplate = $$$"""{{ value_json.sensors.{{{AttributeName.ToLower()}}} }}""",
                StateClass = "measurement"
            }));
        }
        return Task.FromResult<HASensor>(null);
    }
}
