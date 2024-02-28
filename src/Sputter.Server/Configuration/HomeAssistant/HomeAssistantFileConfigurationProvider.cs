using System.IO;
using System.Text.Json;

namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantFileConfigurationProvider(FileConfigurationSource source) : FileConfigurationProvider(source) {
    public override void Load(Stream stream) {
        try {
            Console.WriteLine($"Loading HA addon configuration from file");
            var content = JsonSerializer.Deserialize<HomeAssistantConfigurationSchema>(stream, HomeAssistantSchemaContext.Default.HomeAssistantConfigurationSchema);
            if (content != null) {
                Console.WriteLine(JsonSerializer.Serialize(content, HomeAssistantSchemaContext.Default.HomeAssistantConfigurationSchema));
                var dict = AddonConfigurationLoader.LoadConfiguration(content);
                Data = dict;
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
