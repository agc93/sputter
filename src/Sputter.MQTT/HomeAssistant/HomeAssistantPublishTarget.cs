using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sputter.Core;
using System.Text.Json;

namespace Sputter.MQTT.HomeAssistant;

public class HomeAssistantPublishTarget : IPublishTarget {
    private readonly MQTTConfiguration? _config;
    private readonly ILogger<HomeAssistantPublishTarget>? _logger;

    public bool DebugMode { get; set; }

    public HomeAssistantPublishTarget() { }

    public HomeAssistantPublishTarget(MQTTConfiguration config) : this() {
        _config = config;
    }

    public HomeAssistantPublishTarget(IOptionsSnapshot<MQTTConfiguration> config, ILogger<HomeAssistantPublishTarget> logger) : this() {
        _config = config.Value;
        _logger = logger;
    }

    private bool UnifiedMode => _config?.HomeAssistant?.SingleDeviceMode == true;

    private HomeAssistantDevicePayload GetDriveDevice(DriveEntity drive) {
        var device = new HomeAssistantDevicePayload([drive.Id, $"drv-{drive.UniqueId.SerialNumber}"], drive.Name ?? drive.UniqueId.SerialNumber) {
            Model = drive.Model ?? drive.UniqueId.ModelNumber,
            SuggestedArea = _config?.HomeAssistant?.DeviceArea,
        };
        if (_config?.HomeAssistant is { ExtendedDriveInfo: true }) {
            device.SoftwareVersion = drive.SoftwareVersion;
            device.Manufacturer = drive.Manufacturer;
        }
        if (_config?.HomeAssistant?.OverrideManufacturer == true) {
            device.Manufacturer = "Sputter";
        }
        return device;
    }

    private HomeAssistantDevicePayload GetSputterDevice() {
        var coreDevice = new HomeAssistantDevicePayload([$"sputter-{System.Environment.MachineName}"], "Sputter") {
            SuggestedArea = _config?.HomeAssistant?.DeviceArea
        };
        if (_config?.HomeAssistant?.OverrideManufacturer == true) {
            coreDevice.Manufacturer = "Sputter";
        }
        return coreDevice;
    }

    public async Task<Result> PublishMeasurement(MeasurementResult measurementResult) {
        _logger?.LogTrace("HA Publish Target loaded for {DriveId}", measurementResult.Drive.UniqueId);
        if (_config?.EnableHomeAssistant == true) {
            _logger?.LogDebug("Running HA Discovery publisher for {DriveId}", measurementResult.Drive.UniqueId);
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
            _logger?.LogTrace("Loaded {SensorCount} sensors for drive from {SensorsCount}/{StateCount} measurements", sensors.Count, measurementResult.Measurement.Sensors.Count, measurementResult.Measurement.States.Count);
            var device = UnifiedMode ? GetSputterDevice() : GetDriveDevice(drive);
            if (_config?.Server != null) {
                _logger?.LogTrace("Preparing sensors for {Server} in {DeviceMode} mode", _config.Server, UnifiedMode ? "Unified" : "per-device");
                var mqtt = new MQTTMessageHandler(_config);
                using var client = mqtt.BuildClient();
                var results = new List<Result>();
                foreach (var sensor in sensors) {
                    if (UnifiedMode) {
                        sensor.Value.DeviceName = drive.Name ?? $"{drive.UniqueId.SerialNumber} {sensor.Value.FriendlyName}";
                    }
                    var payload = sensor.Value.ToDiscoveryPayload(device, topic, _config?.HomeAssistant?.EnableAllSensors == true);
                    if (_config?.HomeAssistant?.ExpireAfter is var expiry && expiry != null && expiry > 0) {
                        payload.ExpiryTimeSeconds = expiry;
                    }
                    var json = JsonSerializer.Serialize(payload, SourceGenerationContext.Default.HomeAssistantDiscoveryPayload);
                    var res = await client.SendPayload(sensor.Key, json);
                    results.Add(res);
                }
                _logger?.LogDebug("Published {SuccessCount}/{TotalCount} discovery messages", results.Count(r => r.IsSuccess), sensors.Count);
                return results.All(r => r.IsSuccess) ? Result.Ok() : Result.Fail("At least one sensor failed to be published!");

            } else {
                Console.WriteLine("No MQTT configuration available, skipping publishing!");
                foreach (var sensor in sensors) {
                    var payload = sensor.Value.ToDiscoveryPayload(device, topic);
                    Console.WriteLine($"payload for {sensor.Key}");
                    var json = JsonSerializer.Serialize(payload, SourceGenerationContext.Default.HomeAssistantDiscoveryPayload);
                    _logger?.LogDebug("Generating HA Discovery payload for {SensorName}: {Payload}", sensor.Key, json);
                }
                return Result.Ok();
            }
        }

        return Result.Ok();
    }
}
