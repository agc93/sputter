using Humanizer;
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
            req.AdditionalAdapters.Add(new ScrutinyApiAdapter(scrutinyOpts, null, null));
        }
        var res = await mediator.Send(req);
        PrintResults(res);
        var notif = new DriveMeasurementNotification(res) {
            Targets = publishers
        };
        if (settings.MQTT != null) {
            notif.Targets = publishers.Concat([new MQTTPublishTarget(Options.Create(settings.MQTT))]);
        }
        await mediator.Publish(notif);
        return 0;
    }

    private void PrintResults(IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements) {
        var table = new Table();
        table.AddColumn("Drive");
        table.AddColumn(new TableColumn(new Markup("[grey]Serial[/]")));
        table.AddColumn(new TableColumn(new Markup("Information")));
        table.AddColumn(new TableColumn(new Markup("[darkred_1]Temperature[/]")));
        table.AddColumn(new TableColumn(new Markup("States")));

        foreach (var measure in measurements) {
            var propsTable = new Table();
            propsTable.AddColumn("Property").AddColumn("Value").HideHeaders();
            if (measure.Value != null) {
                propsTable.AddIfSet(nameof(measure.Key.Model), measure.Key.Model);
                propsTable.AddIfSet(nameof(measure.Key.Name), measure.Key.Name);
                if (measure.Key.Capacity != null && double.TryParse(measure.Key.Capacity, out var d)) { 
                    propsTable.AddIfSet("Size", d.Bytes().ToString("#.#"));
                }
                var temp = measure.Value.Sensors.FirstOrDefault(s => s.AttributeName == DriveAttributes.Temperature);
                table.AddRow(
                    new Markup(measure.Key.Id),
                    new Markup(measure.Key.UniqueId.SerialNumber),
                    propsTable,
                    new Panel(temp == null ? "???" : temp.Value.ToString("N2") + temp.Units),
                    new Markup(string.Join(Environment.NewLine, measure.Value.States.Select(s => $"{(s.FriendlyName ?? s.AttributeName)}: [steelblue3]{s.Value}[/]")))
                    );
            }
        }
        AnsiConsole.Write(table);
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
