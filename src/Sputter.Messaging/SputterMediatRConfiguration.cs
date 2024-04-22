namespace Sputter.Messaging;

public class SputterMediatRConfiguration {

	public SputterMediatRConfiguration EnableAggregation(bool enableAggregation = true) {
		AddAggregator = enableAggregation;
		return this;
	}

	public SputterMediatRConfiguration DisableDefaultImplementations(bool disableDefaults = true) {
		EnableDefaults = !disableDefaults;
		return this;
	}

	internal bool AddAggregator { get; set; }
	internal bool EnableDefaults { get; set; } = true;
	public bool AsSingletons { get; set; } = true;
}
