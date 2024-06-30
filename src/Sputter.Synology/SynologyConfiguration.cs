namespace Sputter.Synology;

public class SynologyConfiguration {
	public string? Host { get; set; }
	public string? User { get; set; }
	public string? Password { get; set; }
	public int CacheLifetime { get; set; } = 60;
}