namespace Sputter.Server.Configuration;

public class CachingConfiguration {
    public int Lifetime { get; set; } = 120;
	public int AdapterTimeout { get; set; } = 60;
}
