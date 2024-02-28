using System.Text.Json;

namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantAddonConfigurationProvider(string filePath) : ConfigurationProvider {
    public override void Load() {
        if (filePath != null && File.Exists(filePath)) {
            try {
                var str = File.OpenRead(filePath);
                Console.WriteLine($"Loading HA addon configuration from file");
                var content = JsonSerializer.Deserialize<HomeAssistantConfigurationSchema>(str, HomeAssistantSchemaContext.Default.HomeAssistantConfigurationSchema);
                if (content != null) {
                    Console.WriteLine(JsonSerializer.Serialize(content, HomeAssistantSchemaContext.Default.HomeAssistantConfigurationSchema));
                    var dict = AddonConfigurationLoader.LoadConfiguration(content);
                    Data = dict;
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
        base.Load();
    }
}
