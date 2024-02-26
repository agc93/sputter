using Sputter.Core;
using Sputter.Server.Configuration;
using Microsoft.Extensions.Options;

namespace Sputter.Server;

public class DriveSpecificationReader {
    private readonly IOptionsMonitor<ServerConfiguration> _config;
    private readonly ILogger<DriveSpecificationReader>? _logger;
    private readonly FilterTemplateParser _parser;

    public DriveSpecificationReader(IOptionsMonitor<ServerConfiguration> serverConfig, ILogger<DriveSpecificationReader>? logger, FilterTemplateParser parser) {
        _config = serverConfig;
        _logger = logger;
        _parser = parser;
    }

    public List<DiscoveryTemplate> GetSpecifications() {
        
        var config = _config.CurrentValue;
        var configDrives = config?.Drives ?? [];
        var specs = configDrives.Select(s => {
            return _parser.ParseTemplate(s);
        }).Where(r => r != null).Cast<DiscoveryTemplate>().ToList();
        if (_logger != null && _logger.IsEnabled(LogLevel.Debug)) {
            foreach (var item in specs) {
                _logger.LogDebug("Registered drive template for discovery: '{item}'", item.ToString());
            }
        }
        return specs;
    }

    
}
