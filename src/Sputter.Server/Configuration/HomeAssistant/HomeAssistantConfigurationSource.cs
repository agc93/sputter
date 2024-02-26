using Sputter.Messaging;

namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantConfigurationSource : FileConfigurationSource {
    public override IConfigurationProvider Build(IConfigurationBuilder builder) {
        EnsureDefaults(builder);
        return new HomeAssistantConfigurationProvider(this);
    }
}
