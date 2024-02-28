namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantAddonConfigurationSource(string? filePath = null) : IConfigurationSource {
    public string FilePath { get; set; } = filePath ?? "/data/options.json";
    public IConfigurationProvider Build(IConfigurationBuilder builder) {
        return new HomeAssistantAddonConfigurationProvider(FilePath);
    }
}
