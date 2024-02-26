namespace Sputter.Scrutiny;

public class ScrutinyConfiguration {
    public required string ApiBaseAddress { get; set; }
    public int CacheLifetime { get; set; } = 60;
}
