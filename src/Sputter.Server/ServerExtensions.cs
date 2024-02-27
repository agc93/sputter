using MediatR;
using Sputter.Core;
using Sputter.Messaging;
using System.Text.Json;

namespace Sputter.Server;

internal static class ServerExtensions {

    //internal static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions() {
    //    WriteIndented = true,
    //};

 //   internal static TObject DumpToConsole<TObject>(this TObject @object) {
 //       var output = "NULL";
 //       if (@object != null) {
 //           output = JsonSerializer.Serialize(@object, ServerExtensions.JsonOptions);
 //       }

 //       Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
 //       return @object;
 //   }

	//internal static string DumpToString<TObject>(this TObject @object)
	//{
	//	var output = "NULL";
	//	if (@object != null)
	//	{
	//		output = JsonSerializer.Serialize(@object, ServerExtensions.JsonOptions);
	//	}

 //       return output;
	//}

    internal static async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> MeasureDrives(this IMediator mediator, string? filter, CancellationToken? cancellationToken = null, bool publishMeasurements = true, bool throwOnNoDrives = false) {
        var stopToken = cancellationToken ?? CancellationToken.None;
        //var template = new DiscoveryTemplate(filter ?? "*");
        //return await mediator.MeasureDrives([template], stopToken, publishMeasurements, throwOnNoDrives);
        var driveReq = new DriveDiscoveryRequest() { DriveFilter = filter };
        var drives = await mediator.Send(driveReq, stopToken);
        if (drives == null && throwOnNoDrives) throw new Core.Diagnostics.DrivesNotFoundException();
        var req = new DriveMeasurementRequest(drives!);
        var res = await mediator.Send(req, stopToken);
        if (publishMeasurements) {
            var notif = new DriveMeasurementNotification(res);
            await mediator.Publish(notif, stopToken);
        }
        return res;
    }

    internal static async Task<IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>>> MeasureDrives(this IMediator mediator, IEnumerable<DiscoveryTemplate> templates, CancellationToken? cancellationToken = null, bool publishMeasurements = true, bool throwOnNoDrives = false) {
        var stopToken = cancellationToken ?? CancellationToken.None;
        var driveReq = new DriveDiscoveryRequest() { Templates = templates.ToList() };
        var drives = await mediator.Send(driveReq, stopToken);
        if (drives == null && throwOnNoDrives) throw new Core.Diagnostics.DrivesNotFoundException();
        var req = new DriveMeasurementRequest(drives!);
        var res = await mediator.Send(req, stopToken);
        if (publishMeasurements) {
            var notif = new DriveMeasurementNotification(res);
            await mediator.Publish(notif, stopToken);
        }
        return res;
    }
}