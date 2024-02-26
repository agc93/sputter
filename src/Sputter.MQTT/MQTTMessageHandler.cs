using Microsoft.Extensions.Options;
using Sputter.Core;
using System.Text.Json;

namespace Sputter.MQTT;
public class MQTTMessageHandler(MQTTConfiguration config) {
    private readonly MQTTConfiguration _config = config;

    public static string ToStateMessage(DriveMeasurement measurement) {
        var payload = new MQTTStatePayload {
            Sensors = measurement.Sensors.ToDictionary(s => (s.FriendlyName ?? s.AttributeName).ToLower(), v => v.Value),
            States = measurement.States.ToDictionary(s => (s.FriendlyName ?? s.AttributeName).ToLower(), v => v.Value)
        };
        return JsonSerializer.Serialize(payload, SourceGenerationContext.Default.MQTTStatePayload);
    }

    internal MQTTClient BuildClient() {
        var (url, port) = _config.Parse();
        if (!string.IsNullOrWhiteSpace(_config.UserName) && !string.IsNullOrWhiteSpace(_config.Password)) {
            return new MQTTClient(url, port, _config.UserName, _config.Password);
        }
        var client = new MQTTClient(url, port);
        return client;
    }
}
