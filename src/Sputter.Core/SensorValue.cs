namespace Sputter.Core;

public abstract class SensorValue<T> {
	public string? FriendlyName { get; set; }
	public required string AttributeName { get; init; }
	public required T Value { get; set; }
}
