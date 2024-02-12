using Microsoft.Extensions.Options;
using Sputter.Core;
using System.Text.Json;

namespace Sputter.MQTT;
public class MQTTMessageHandler(IOptions<MQTTConfiguration> config) {
    private readonly IOptions<MQTTConfiguration> _config = config;

    public static string ToStateMessage(DriveMeasurement measurement) {
        var payload = new MQTTStatePayload {
            Sensors = measurement.Sensors.ToDictionary(s => (s.FriendlyName ?? s.AttributeName).ToLower(), v => v.Value),
            States = measurement.States.ToDictionary(s => (s.FriendlyName ?? s.AttributeName).ToLower(), v => v.Value)
        };
        return JsonSerializer.Serialize(payload, SourceGenerationContext.Default.MQTTStatePayload);
    }

    internal MQTTClient BuildClient() {
        (string, int?) serverUrl = _config.Value.Server.Split(':') is var parts && parts.Length > 1 && int.TryParse(parts[1], out var port)
            ? (parts[0], port)
            : (parts[0], null);
        if (!string.IsNullOrWhiteSpace(_config.Value.UserName) && !string.IsNullOrWhiteSpace(_config.Value.Password)) {
            return new MQTTClient(serverUrl.Item1, serverUrl.Item2 ?? _config.Value.Port, _config.Value.UserName, _config.Value.Password);
        }
        var client = new MQTTClient(serverUrl.Item1, serverUrl.Item2 ?? _config.Value.Port);
        return client;
    }
}
