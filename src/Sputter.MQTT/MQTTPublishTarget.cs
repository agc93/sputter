using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sputter.Core;

namespace Sputter.MQTT;

public class MQTTPublishTarget : IPublishTarget {
	private readonly Func<MQTTConfiguration?> _config;
	private readonly ILogger<MQTTPublishTarget>? _logger;

	public MQTTPublishTarget() {
		_config = () => null;
	}

	public MQTTPublishTarget(IOptions<MQTTConfiguration> config) : this() {
		_config = () => config.Value;
	}

	public MQTTPublishTarget(IOptionsSnapshot<MQTTConfiguration>? monitor, ILogger<MQTTPublishTarget>? logger) : this() {
		_config = monitor == null
			? () => null
			: () => monitor.Value;
		_logger = logger;
	}

	public async Task<Result> PublishMeasurement(MeasurementResult result) {
		_logger?.LogTrace("Running MQTT publishing target for {DriveId}", result.Drive.UniqueId);
		var topic = result.Drive.ToStateTopic();
		var payload = MQTTMessageHandler.ToStateMessage(result.Measurement);
		var conf = _config();
		if (conf != null) {
			_logger?.LogTrace("Connecting to MQTT server {Server} to publish measurement", conf.Server);
			var mqtt = new MQTTMessageHandler(conf);
			using var client = mqtt.BuildClient();
			var res = await client.SendPayload(topic, payload);
			_logger?.LogTrace("Published measurement payload to MQTT: {Result}", res.IsSuccess);
			return res;
		} else {
			Console.WriteLine("No MQTT configuration available, skipping publishing!");
			return Result.Ok();
		}
	}
}
