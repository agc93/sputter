namespace Sputter.MQTT.HomeAssistant;

public class HomeAssistantSensorDetails(string uniqueId) {
    public string UniqueId { get; } = uniqueId;
    public string? DeviceClass { get; set; }
    public string? DeviceName { get; set; }
    public string? StateClass { get; set; }
    public string? Units { get; set; }
    public string? ValueTemplate { get; set; }
    public (string? On, string? Off) StatePayload { get; set; }
    public string? FriendlyName { get; set; }

    public HomeAssistantDiscoveryPayload ToDiscoveryPayload(HomeAssistantDevicePayload device, string stateTopic, bool forceEnable = false) {
        var payload = new HomeAssistantDiscoveryPayload(device, stateTopic) {
            StateClass = StateClass,
            DeviceClass = DeviceClass,
            Name = DeviceName,
            UniqueId = UniqueId,
            Unit = Units,
            ValueTemplate = ValueTemplate,
            OnStatePayload = StatePayload.On,
            OffStatePayload = StatePayload.Off,
            EnabledByDefault = forceEnable || StateClass == "measurement",
        };
        return payload;
    }
}