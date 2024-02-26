using Sputter.Server.Configuration.HomeAssistant;

namespace Microsoft.Extensions.Configuration;

public static class HomeAssistantConfigurationSchemaExtensions {
    public static IConfigurationBuilder AddHomeAssistantAddOnConfiguration
    (this IConfigurationBuilder builder, string? filePath = null) {
        var source = string.IsNullOrWhiteSpace(filePath)
            ? new HomeAssistantConfigurationSource { Path = "/data/options.json", Optional = true, ReloadOnChange = true }
            : new HomeAssistantConfigurationSource { Path = filePath, Optional = true, ReloadOnChange = true };
        return builder.Add(source);
    }
}
