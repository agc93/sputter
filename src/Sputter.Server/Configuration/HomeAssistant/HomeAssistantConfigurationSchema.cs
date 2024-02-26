namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantConfigurationSchema {
    public string? Broker { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }
    public int? AutoMeasureInterval { get; set; }
    public List<string> DriveTemplates { get; set; } = [];
    public string? ScrutinyApiAddress { get; set; }
    public bool? SingleDeviceMode { get; set; }
    public string? DeviceArea { get; set; }
    public int? ExpireAfter { get; set; }
    public bool? EnableAllDrives { get; set; }
}
