using Sputter.Messaging;
using Sputter.Server.Configuration;
using System.IO.Hashing;
using System.Text;
using System.Text.Json;

namespace Sputter.Server;

internal static class CacheExtensions {
    internal static string GetChecksum(this DriveDiscoveryRequest request) {
        var json = JsonSerializer.SerializeToUtf8Bytes(request, ChecksumJsonContext.Default.DriveDiscoveryRequest);
        return GetChecksumString(json);
    }

    internal static string GetChecksum(this DriveMeasurementRequest request) {
        var json = JsonSerializer.SerializeToUtf8Bytes(request, ChecksumJsonContext.Default.DriveMeasurementRequest);
        return GetChecksumString(json);
    }

    internal static byte[] GetChecksumBytes(this ServerConfiguration config) {
        var json = JsonSerializer.SerializeToUtf8Bytes(config, ChecksumJsonContext.Default.ServerConfiguration);
        return GetChecksumBytes(json);
    }

    internal static byte[] GetChecksumBytes(this MQTT.MQTTConfiguration config) {
        var json = JsonSerializer.SerializeToUtf8Bytes(config, ChecksumJsonContext.Default.MQTTConfiguration);
        return GetChecksumBytes(json);
    }

    internal static string GetChecksumString(byte[] json) {
        byte[] hash = GetChecksumBytes(json);
        //var strContents = Encoding.UTF8.GetString(hash);
        var strContents = Convert.ToBase64String(hash);
        //var str = hash.ToString();
        return strContents;
    }

    private static byte[] GetChecksumBytes(byte[] json) {
        var xx = new XxHash64();
        xx.Append(json);
        var hash = xx.GetHashAndReset();
        return hash;
    }
}
