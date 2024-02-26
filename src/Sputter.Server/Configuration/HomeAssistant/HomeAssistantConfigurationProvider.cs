using Sputter.MQTT;
using Sputter.Scrutiny;
using System.Text.Json;

namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantConfigurationProvider(FileConfigurationSource source) : FileConfigurationProvider(source) {

    private static void LoadIfSet<T>(IDictionary<string, string?> dict, T? confValue, string key) {
        if (confValue != null) {
            dict.TryAdd(key, confValue.ToString());
        }
    }

    private static void LoadCollectionIfSet<T>(IDictionary<string, string?> dict, IEnumerable<T>? confValue, string key) {
        var elementsCount = (confValue ?? []).Count();
        if (confValue != null && elementsCount > 0) {
            for (var i = 0; i < elementsCount; i++) {
                dict.TryAdd($"{key}:{i}", confValue.ElementAt(i)!.ToString());
            }
        }
    }

    public override void Load(Stream stream) {
        try {
            var content = JsonSerializer.Deserialize<HomeAssistantConfigurationSchema>(stream, HomeAssistantSchemaContext.Default.HomeAssistantConfigurationSchema);
            if (content != null) {
                var dict = new Dictionary<string, string?>(StringComparer.Ordinal);
                LoadIfSet(dict, content.AutoMeasureInterval, $"Sputter:{nameof(ServerConfiguration.AutoMeasureInterval)}");
                LoadIfSet(dict, content.Broker, $"MQTT:{nameof(MQTTConfiguration.Server)}");
                LoadIfSet(dict, content.UserName, $"MQTT:{nameof(MQTTConfiguration.UserName)}");
                LoadIfSet(dict, content.Password, $"MQTT:{nameof(MQTTConfiguration.Password)}");
                LoadIfSet(dict, content.Port, $"MQTT:{nameof(MQTTConfiguration.Port)}");
                LoadIfSet(dict, content.ScrutinyApiAddress, $"Scrutiny:{nameof(ScrutinyConfiguration.ApiBaseAddress)}");
                LoadIfSet(dict, content.SingleDeviceMode, $"MQTT:{nameof(MQTTConfiguration.HomeAssistant)}:{nameof(MQTTConfiguration.HomeAssistant.SingleDeviceMode)}");
                LoadIfSet(dict, content.DeviceArea, $"MQTT:{nameof(MQTTConfiguration.HomeAssistant)}:{nameof(MQTTConfiguration.HomeAssistant.DeviceArea)}");
                LoadIfSet(dict, content.ExpireAfter, $"MQTT:{nameof(MQTTConfiguration.HomeAssistant)}:{nameof(MQTTConfiguration.HomeAssistant.ExpireAfter)}");
                LoadIfSet(dict, content.EnableAllDrives, $"MQTT:{nameof(MQTTConfiguration.HomeAssistant)}:{nameof(MQTTConfiguration.HomeAssistant.EnableAllSensors)}");
                LoadCollectionIfSet(dict, content.DriveTemplates, $"Sputter:{nameof(ServerConfiguration.Drives)}");
                Data = dict;
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
