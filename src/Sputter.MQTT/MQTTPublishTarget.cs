using FluentResults;
using Microsoft.Extensions.Options;
using Sputter.Core;

namespace Sputter.MQTT;

public class MQTTPublishTarget : IPublishTarget {
    private readonly Func<MQTTConfiguration?> _config;

    public MQTTPublishTarget() {
        _config = () => null;
    }

    public MQTTPublishTarget(IOptions<MQTTConfiguration> config) : this() {
        _config = () => config.Value;
    }

    public MQTTPublishTarget(IOptionsSnapshot<MQTTConfiguration>? monitor) : this() {
		_config = monitor == null
			? () => null
			: () => monitor.Value;
    }

    public async Task<Result> PublishMeasurement(MeasurementResult result) {
        var topic = result.Drive.ToStateTopic();
        var payload = MQTTMessageHandler.ToStateMessage(result.Measurement);
        var conf = _config();
        if (conf != null) {
            var mqtt = new MQTTMessageHandler(conf);
            using var client = mqtt.BuildClient();
            var res = await client.SendPayload(topic, payload);
            return res;
        } else {
            Console.WriteLine("No MQTT configuration available, skipping publishing!");
            return Result.Ok();
        }
    }
}
