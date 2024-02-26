namespace Sputter.Server.Configuration;

public class ServerConfiguration {
    //DO NOT MIX configuration sources when loading templates!
    // Because MS have made some real funky decisions, mixing config sources (like
    // using the HA addon provider and an `appsettings.json`) will blend templates
    // from configured sources, not merge them or even replace them. This ends up
    // doing very weird things. https://github.com/dotnet/runtime/issues/36569#issuecomment-1107003676
    public List<string>? Drives { get; set; } = [];
    public int? AutoMeasureInterval { get; set; }
    public bool AllowPublishingAll { get; set;} = false;
}
