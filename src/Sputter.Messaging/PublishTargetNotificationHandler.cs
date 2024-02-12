using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sputter.Core;

namespace Sputter.Messaging;

public class PublishTargetNotificationHandler(IEnumerable<IPublishTarget> publishTargets) : INotificationHandler<DriveMeasurementNotification> {
    private readonly List<IPublishTarget> _publishTargets = publishTargets.ToList();

    public async Task Handle(DriveMeasurementNotification notification, CancellationToken cancellationToken) {
        var targets = notification.Targets ?? _publishTargets;
        var service = new DriveMeasurementService([], targets);
        var res = await service.PublishMeasurementsResultsAsync(notification.Measurements).WaitForAll();
        //foreach (var uniqueDrive in notification.Measurements) {
        //    if (uniqueDrive.Value != null) {
        //        foreach (var publisher in targets) {
        //            try {
        //                var res = await publisher.PublishMeasurement(new MeasurementResult(uniqueDrive.Key, uniqueDrive.Value!));
        //                if (res.IsSuccess) {
        //                    Console.WriteLine($"Succesfully published '{uniqueDrive.Key.UniqueId}' to '{publisher.GetType().Name}'");
        //                } else {
        //                    Console.WriteLine($"Failed to publish measurement for '{uniqueDrive.Key.UniqueId}' to '{publisher.GetType().Name}'");
        //                }
        //            } catch {
        //                Console.WriteLine($"Error encountered while publishing to '{publisher.GetType().Name}'");
        //            }
        //        }
        //    }
        //}
    }
}
