using Sputter.Server.Configuration.HomeAssistant;

namespace Microsoft.Extensions.Configuration;

public static class HomeAssistantConfigurationSchemaExtensions {
    public static IConfigurationBuilder AddHomeAssistantAddOnConfiguration
    (this IConfigurationBuilder builder, string? filePath = null, bool? enableContainerCompatibility = null) {
        var enableCompatibility = enableContainerCompatibility ?? 
            Environment.GetEnvironmentVariable("SPUTTER_DYNAMIC_CONFIG") is var dynamicMode
                && !(bool.TryParse(dynamicMode, out var parsed) && parsed);
        IConfigurationSource source = enableCompatibility
            ? new HomeAssistantAddonConfigurationSource(filePath)
            : new HomeAssistantFileConfigurationSource() {
                Path = filePath ?? "/data/options.json",
                Optional = true,
                ReloadOnChange = true,
                OnLoadException = ex => {
                    Console.WriteLine(ex.ToString());
                },
                ReloadDelay = 1000
            };
        return builder.Add(source);
    }
}
