using MediatR;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;
using Sputter.Core;
using Sputter.Messaging;
using Sputter.MQTT;
using Sputter.Scrutiny;

namespace Sputter.Console;
public class MeasureCommand : AsyncCommand<MeasureCommand.Settings> {
    private readonly IMediator mediator;
    private readonly IEnumerable<IPublishTarget> publishers;

    public MeasureCommand(IMediator mediator, IEnumerable<IPublishTarget> publishers) {
        this.mediator = mediator;
        this.publishers = publishers;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
        var req = new DriveMeasurementRequest(settings.DriveFilter);
        if (!string.IsNullOrWhiteSpace(settings.ScrutinyApiAddress)) {
            var scrutiny = new ScrutinyConfiguration {
                ApiBaseAddress = settings.ScrutinyApiAddress
            };
            var scrutinyOpts = Options.Create(scrutiny);
            req.AdditionalAdapters.Add(new ScrutinyApiAdapter(scrutinyOpts));
        }
        var res = await mediator.Send(req);
        var notif = new DriveMeasurementNotification(res) {
            Targets = publishers
        };
        if (settings.MQTT != null) {
            notif.Targets = publishers.Concat([new MQTTPublishTarget(Options.Create(settings.MQTT))]);
        }
        await mediator.Publish(notif);
        return 0;
    }

    public sealed class Settings : SputterSettings {

        public override ValidationResult Validate() {
            if (!string.IsNullOrWhiteSpace(MQTTServer)) {
                MQTT = new MQTTConfiguration { Server = MQTTServer };

                if (!string.IsNullOrWhiteSpace(MQTTCredentials) && MQTTCredentials.Split(':') is var parts && parts.Length > 1) {
                    MQTT.UserName = parts[0];
                    MQTT.Password = parts[1];
                }
            }
            return base.Validate();
        }

        [CommandOption("--mqtt-server")]
        public string? MQTTServer { get; set; }
        [CommandOption("--mqtt-credentials")]
        public string? MQTTCredentials { get; set; }

        internal MQTTConfiguration? MQTT { get; set; }
    }
}
