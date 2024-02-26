namespace Sputter.MQTT.HomeAssistant;

public class HomeAssistantConfiguration {
    public string? DeviceArea { get; set; }
    public bool? ExtendedDriveInfo { get; set; }
    public string? DeviceIdPrefix { get; set; }
    public bool? SingleDeviceMode { get; set; }
    public bool? OverrideManufacturer { get; set; }
    public bool? EnableAllSensors { get; set; }
    public int? ExpireAfter { get; set; }
}
