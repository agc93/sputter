using FluentResults;
using Microsoft.Extensions.Options;
using Sputter.Core;
using System.Text.Json;

namespace Sputter.MQTT.HomeAssistant;

public class HomeAssistantPublishTarget : IPublishTarget {
    private readonly IOptions<MQTTConfiguration>? _config;

    public HomeAssistantPublishTarget() { }

    public HomeAssistantPublishTarget(IOptions<MQTTConfiguration> config) : this() {
        _config = config;
    }

    private bool UnifiedMode => _config?.Value.HomeAssistant?.SingleDeviceMode == true;

    private HomeAssistantDevicePayload GetDriveDevice(DriveEntity drive) {
        var device = new HomeAssistantDevicePayload([drive.Id, $"drv-{drive.UniqueId.SerialNumber}"], drive.Name ?? drive.UniqueId.SerialNumber) {
            Model = drive.Model ?? drive.UniqueId.ModelNumber,
            SuggestedArea = _config?.Value.HomeAssistant?.DeviceArea,
        };
        if (_config?.Value.HomeAssistant is { ExtendedDriveInfo: true }) {
            device.SoftwareVersion = drive.SoftwareVersion;
            device.Manufacturer = drive.Manufacturer;
        }
        if (_config?.Value.HomeAssistant?.OverrideManufacturer == true) {
            device.Manufacturer = "Sputter";
        }
        return device;
    }

    private HomeAssistantDevicePayload GetSputterDevice() {
        var coreDevice = new HomeAssistantDevicePayload([$"sputter-{System.Environment.MachineName}"], "Sputter") {
            SuggestedArea = _config?.Value.HomeAssistant?.DeviceArea
        };
        if (_config?.Value.HomeAssistant?.OverrideManufacturer == true) {
            coreDevice.Manufacturer = "Sputter";
        }
        return coreDevice;
    }

    public async Task<Result> PublishMeasurement(MeasurementResult measurementResult) {
        var measure = measurementResult.Measurement;
        var drive = measurementResult.Drive;
        var topic = drive.ToStateTopic();
        var extractors = new List<ISensorExtractor> { new TemperatureSensorExtractor(), new HealthyStateExtractor() };
        var sensors = new Dictionary<string, HomeAssistantSensorDetails>();
        foreach (var extractor in extractors) {
            if (measure != null && extractor.Supports(measure)) {
                var result = new MeasurementResult(measurementResult.Drive, measure);
                var sensor = await extractor.BuildSensor(result);
                if (sensor.HasValue) { sensors.Add(sensor.Value.Key, sensor.Value.Value); }
            }
        }
        var device = UnifiedMode ? GetSputterDevice() : GetDriveDevice(drive);
        if (_config != null) {
            var mqtt = new MQTTMessageHandler(_config);
            using var client = mqtt.BuildClient();
            var results = new List<Result>();
            foreach (var sensor in sensors) {
                if (UnifiedMode) {
                    sensor.Value.DeviceName = drive.Name ?? $"{drive.UniqueId.SerialNumber} {sensor.Value.FriendlyName}";
                }
                var payload = sensor.Value.ToDiscoveryPayload(device, topic);
                var json = JsonSerializer.Serialize(payload, SourceGenerationContext.Default.HomeAssistantDiscoveryPayload);
                var res = await client.SendPayload(sensor.Key, json);
                results.Add(res);
            }
            return results.All(r => r.IsSuccess) ? Result.Ok() : Result.Fail("At least one sensor failed to be published!");

        } else {
            Console.WriteLine("No MQTT configuration available, skipping publishing!");
            foreach (var sensor in sensors) {
                var payload = sensor.Value.ToDiscoveryPayload(device, topic);
                Console.WriteLine($"payload for {sensor.Key}");
                var json = JsonSerializer.Serialize(payload, SourceGenerationContext.Default.HomeAssistantDiscoveryPayload);
                Console.WriteLine(json);
            }
            return Result.Ok();
        }
    }
}
