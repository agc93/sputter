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

	public SputterMediatRConfiguration EnableRounding(bool enableRounding = true) {
		RoundMeasurements = enableRounding;
		return this;
	}

	internal bool AddAggregator { get; set; }
	internal bool RoundMeasurements { get; set; } = true;
	internal bool EnableDefaults { get; set; } = true;
	public bool AsSingletons { get; set; } = true;
}
