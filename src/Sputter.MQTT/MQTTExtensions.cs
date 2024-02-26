using MQTTnet;
using MQTTnet.Client;
using Sputter.Core;

namespace Sputter.MQTT;

public static class MQTTExtensions {
    internal static string ToTopicSegment(this UniqueId id) {
        return $"drv-{id.SerialNumber}";
    }

    internal static string ToStateTopic(this DriveEntity drive) {
        return $"sputter/{drive.UniqueId.ToTopicSegment()}";
    }

    public static (string Url, int? Port) Parse(this MQTTConfiguration config) {
        (string, int?) serverUrl = config.Server.Split(':') is var parts && parts.Length > 1 && int.TryParse(parts[1], out var port)
            ? (parts[0], port)
            : (parts[0], null);
        return (serverUrl.Item1, serverUrl.Item2 ?? config.Port);
    }

    public static async Task<IMqttClient> ConnectAndSubscribe(this IMqttClient mqttClient, MqttFactory mqttFactory, MqttClientOptions? clientOptions, string topic, Func<MqttApplicationMessageReceivedEventArgs, Task>? callback = null, CancellationToken? cancellationToken = null)
    {
		var stopToken = cancellationToken ?? CancellationToken.None;
        if (callback != null) {
            mqttClient.ApplicationMessageReceivedAsync += callback;
        }

		await mqttClient.ConnectAsync(clientOptions, stopToken);

		var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
			.WithTopicFilter(
				f =>
				{
					f.WithTopic(topic);
				})
			.Build();

		await mqttClient.SubscribeAsync(mqttSubscribeOptions, stopToken);
        return mqttClient;
	}
}
