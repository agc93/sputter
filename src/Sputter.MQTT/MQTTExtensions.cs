using Sputter.Core;

namespace Sputter.MQTT;

public static class MQTTExtensions {
    internal static string ToTopicSegment(this UniqueId id) {
        return $"drv-{id.SerialNumber}";
    }

    internal static string ToStateTopic(this DriveEntity drive) {
        return $"sputter/{drive.UniqueId.ToTopicSegment()}";
    }
}
