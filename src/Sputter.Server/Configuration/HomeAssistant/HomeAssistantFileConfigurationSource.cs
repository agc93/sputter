using Sputter.Messaging;

namespace Sputter.Server.Configuration.HomeAssistant;

public class HomeAssistantFileConfigurationSource : FileConfigurationSource {
    public override IConfigurationProvider Build(IConfigurationBuilder builder) {
        EnsureDefaults(builder);
        return new HomeAssistantFileConfigurationProvider(this);
    }
}
