using Sputter.Server.Configuration.HomeAssistant;

namespace Microsoft.Extensions.Configuration;

public static class HomeAssistantConfigurationSchemaExtensions {
    public static IConfigurationBuilder AddHomeAssistantAddOnConfiguration
    (this IConfigurationBuilder builder, string? filePath = null) {
        var source = new HomeAssistantConfigurationSource { 
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
