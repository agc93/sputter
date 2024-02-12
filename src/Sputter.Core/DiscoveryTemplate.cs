namespace Sputter.Core;

public class DiscoveryTemplate(string matchSpec) {
    public string? SourceAdapter { get; set; }
    public string Match { get; set; } = matchSpec;
}
