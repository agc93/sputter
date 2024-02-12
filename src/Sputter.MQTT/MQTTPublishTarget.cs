using FluentResults;
using Microsoft.Extensions.Options;
using Sputter.Core;

namespace Sputter.MQTT;

public class MQTTPublishTarget : IPublishTarget {
    private readonly IOptions<MQTTConfiguration>? _config;

    public MQTTPublishTarget() {

    }

    public MQTTPublishTarget(IOptions<MQTTConfiguration> config) : this() {
        _config = config;
    }

    public async Task<Result> PublishMeasurement(MeasurementResult result) {
        var topic = result.Drive.ToStateTopic();
        Console.WriteLine($"Payload for {topic}");
        var payload = MQTTMessageHandler.ToStateMessage(result.Measurement);
        Console.WriteLine(payload);
        if (_config != null) {
            var mqtt = new MQTTMessageHandler(_config);
            using var client = mqtt.BuildClient();
            var res = await client.SendPayload(topic, payload);
            return res;
        } else {
            Console.WriteLine("No MQTT configuration available, skipping publishing!");
            return Result.Ok();
        }
    }
}
