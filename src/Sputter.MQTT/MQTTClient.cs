using FluentResults;
using MQTTnet;
using MQTTnet.Client;

namespace Sputter.MQTT;

/// <summary>
/// Terrible little abstraction over MQTTNet's MqttClient.
/// This arguably shouldn't exist, and might not (eventually).
/// </summary>
internal class MQTTClient : IDisposable {
    private readonly MqttFactory _factory;
    private readonly string _serverUrl;
    private readonly int? _serverPort;
    private readonly bool _enableAuth;
    private readonly (string userName, string password) _credentials;
    private IMqttClient? _client;

    public MQTTClient(string serverUrl, int? serverPort = null, string? userName = null, string? password = null) {
        _factory = new MqttFactory();
        _serverUrl = serverUrl;
        _serverPort = serverPort;
        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password)) {
            _enableAuth = true;
            _credentials = (userName, password);
        }
    }

    public async Task<Result> SendPayload(string topic, string payload) {
        var client = await GetClient();
        var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag()
                .Build();
        var res = await client.PublishAsync(applicationMessage);
        return res.IsSuccess ? Result.Ok() : Result.Fail(res.ReasonString);
    }

    private async Task<IMqttClient> GetClient() {
        _client ??= _factory.CreateMqttClient();
        if (!_client.IsConnected) {
            var builder = new MqttClientOptionsBuilder()
                .WithTcpServer(_serverUrl, _serverPort);
            if (_enableAuth) {
                builder.WithCredentials(_credentials.userName, _credentials.password);
            }
            var mqttClientOptions = builder.Build();
            await _client.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }
        return _client;
    }

    public void Dispose() => _client?.Dispose();
}
