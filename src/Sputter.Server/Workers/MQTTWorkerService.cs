
using MediatR;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Sputter.MQTT;
using System.Text;
using System.Text.Json;

namespace Sputter.Server.Workers;

public class MQTTWorkerService : BackgroundService {
    private readonly ILogger<MQTTWorkerService> _logger;
    private readonly IServiceProvider _services;
    private readonly IMediator _mediator;
    private const string TopicName = "sputter/_cmd";
    private IMqttClient? _mqttClient;
    //private IDisposable? _onChange;

    public MQTTWorkerService(IServiceProvider services, ILogger<MQTTWorkerService> logger, IMediator mediator) {
        _logger = logger;
        _services = services;
        _mediator = mediator;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Starting MQTT subscriptions");
        await SubscribeToTopic(stoppingToken);
    }

    private async Task SubscribeToTopic(CancellationToken stopToken) {
        var connToken = new CancellationTokenSource();
        stopToken.Register(connToken.Cancel);
        var mqttFactory = new MqttFactory();
        await using var scope = _services.CreateAsyncScope();
        var scopedMonitor = scope.ServiceProvider.GetService<IOptionsMonitor<MQTTConfiguration>>();
        var scopedOptions = scope.ServiceProvider.GetService<IOptions<MQTTConfiguration>>();
        if (scopedMonitor != null) {
            async Task ReconnectWithConfiguration(MQTTConfiguration c) {
                _logger.LogInformation("Configuration change detected, reconnecting to MQTT!");
                if (c.Server != null && _mqttClient is { IsConnected: true }) {
                    //_mqttClient.DisconnectedAsync += async e => _logger.LogDebug("Disconnecting MQTT Client during config refresh");
                    await _mqttClient.DisconnectAsync(MqttClientDisconnectOptionsReason.NormalDisconnection, cancellationToken: stopToken);
                    try {
                        var mqttClientOptions = GetClientOptions(c);
                        _mqttClient = await _mqttClient.ConnectAndSubscribe(mqttFactory, mqttClientOptions, TopicName, cancellationToken: stopToken);
                        _logger.LogInformation("Reconnected to MQTT server with new configuration! ({Connected})", _mqttClient.IsConnected);
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Failed to reconnect to MQTT server!");
                    }
                }
            }
            _ = scopedMonitor.OnChangeDedup(c => {
                _ = ReconnectWithConfiguration(c);
            });
        }
        var opts = scopedMonitor == null
            ? scopedOptions?.Value
            : scopedMonitor.CurrentValue;
        


        if (opts?.Server != null) {
            var mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = GetClientOptions(opts);
            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.DisconnectedAsync += async e => {
                if (e.Reason != MqttClientDisconnectReason.NormalDisconnection) {
                    _logger.LogInformation("Disconnected from MQTT server, attempting reconnection...");
                    await Task.Delay(TimeSpan.FromSeconds(opts.ReconnectDelay), stopToken);

                    try {
                        _mqttClient = await mqttClient.ConnectAndSubscribe(mqttFactory, mqttClientOptions, TopicName, cancellationToken: stopToken);
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Failed to reconnect to MQTT server!");
                    }
                } else {
                    _logger.LogInformation("Disconnected from MQTT server, skipping reconnection...");
                }
            };

            _mqttClient = await mqttClient.ConnectAndSubscribe(mqttFactory, mqttClientOptions, TopicName, e1 => OnMessageReceived(e1, stopToken), stopToken);
            
        } else {
            await Task.CompletedTask;
        }
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e, CancellationToken stopToken) {
        _logger.LogDebug($"Received MQTT command message!");
        if (_logger.IsEnabled(LogLevel.Debug)) {
            _logger.LogDebug("MQTT message contents: {Message}", Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
        }
        if (e.ApplicationMessage.PayloadSegment.Count > 0) {
            try {
                var message = JsonSerializer.Deserialize(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment), MQTTWorkerJsonContext.Default.MQTTWorkerCommand);
                if (!string.IsNullOrWhiteSpace(message?.Command)) {
                    switch (message.Command) {
                        case "getLatest":
                            var _ = await _mediator.MeasureDrives("*", stopToken);
                            return;
                        default:
                            _logger.LogWarning("Unknown command message received on MQTT topic!");
                            return;
                    }
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Error parsing MQTT command message!");
            }
        }
    }

    private static MqttClientOptions GetClientOptions(MQTTConfiguration config) {

        var (serverUrl, serverPort) = config.Parse();
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer(serverUrl, serverPort);
        var enableAuth = (!string.IsNullOrWhiteSpace(config.UserName) && !string.IsNullOrWhiteSpace(config.Password));
        if (enableAuth) {
            builder.WithCredentials(config.UserName, config.Password);
        }
        var mqttClientOptions = builder.Build();
        return mqttClientOptions;
    }
}
