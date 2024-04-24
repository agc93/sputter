namespace Sputter.Core;
public static class CoreExtensions {
	// A very simple wildcard match
	// https://github.com/picrap/WildcardMatch

	/// <summary>
	/// Tells if the given string matches the given wildcard.
	/// Two wildcards are allowed: '*' and '?'
	/// '*' matches 0 or more characters
	/// '?' matches any character
	/// </summary>
	/// <param name="wildcard">The wildcard.</param>
	/// <param name="s">The s.</param>
	/// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
	/// <returns></returns>
	public static bool WildcardMatch(this string wildcard, string s, bool ignoreCase = false) {
		return WildcardMatch(wildcard, s, 0, 0, ignoreCase);
	}

	/// <summary>
	/// Internal matching algorithm.
	/// </summary>
	/// <param name="wildcard">The wildcard.</param>
	/// <param name="s">The s.</param>
	/// <param name="wildcardIndex">Index of the wildcard.</param>
	/// <param name="sIndex">Index of the s.</param>
	/// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
	/// <returns></returns>
	private static bool WildcardMatch(this string wildcard, string s, int wildcardIndex, int sIndex, bool ignoreCase) {
		for (; ; )
		{
			// in the wildcard end, if we are at tested string end, then strings match
			if (wildcardIndex == wildcard.Length)
				return sIndex == s.Length;

			var c = wildcard[wildcardIndex];
			switch (c) {
				// always a match
				case '?':
					break;
				case '*':
					// if this is the last wildcard char, then we have a match, whatever the tested string is
					if (wildcardIndex == wildcard.Length - 1)
						return true;
					// test if a match follows
					return Enumerable.Range(sIndex, s.Length - sIndex).Any(i => WildcardMatch(wildcard, s, wildcardIndex + 1, i, ignoreCase));
				default:
					var cc = ignoreCase ? char.ToLower(c) : c;
					if (s.Length == sIndex) {
						return false;
					}
					var sc = ignoreCase ? char.ToLower(s[sIndex]) : s[sIndex];
					if (cc != sc)
						return false;
					break;
			}

			wildcardIndex++;
			sIndex++;
		}
	}

	public static bool MatchesFilter(this DriveEntity entity, string? filter) {
		if (filter == null) return true;
		return filter.WildcardMatch(entity.UniqueId.SerialNumber)
			|| ((!string.IsNullOrWhiteSpace(entity.UniqueId.WWN)) && filter.WildcardMatch(entity.UniqueId.WWN))
			|| ((!string.IsNullOrWhiteSpace(entity.UniqueId.ModelNumber)) && filter.WildcardMatch(entity.UniqueId.ModelNumber));
	}

	public static async Task<List<T>> WaitForAll<T>(this IAsyncEnumerable<T> inputResults) {
		var output = new List<T>();
		await foreach (var result in inputResults) {
			output.Add(result);
		}
		return output;
	}

	public static void AddSource(this DriveMeasurement? measurement, string adapterName) {
		measurement?.AddSources([adapterName]);
	}

	public static void AddSource(this DriveMeasurement? measurement, DriveMeasurement? sourceMeasurement) {
		if (measurement != null && sourceMeasurement != null) {
			var sourceStateValue = sourceMeasurement.States.FirstOrDefault(st => st.AttributeName == DriveAttributes.SourceAdapter);
			if (sourceStateValue != null) {
				measurement.AddSources(sourceStateValue.Value.SplitSources());
			}
		}
	}

	public static void AddSources(this DriveMeasurement? measurement, IEnumerable<string> adapterNames) {
		if (measurement != null) {
			var matching = measurement.States.FirstOrDefault(st => st.AttributeName == DriveAttributes.SourceAdapter);
			if (matching != null) {
				matching.Value = string.Join(';', matching.Value.SplitSources().Concat(adapterNames).Distinct());
			} else {
				measurement.States.Add(
					new DriveState {
						AttributeName = DriveAttributes.SourceAdapter,
						Value = adapterNames.Count() > 1 ? string.Join(';', adapterNames) : adapterNames.First()
					});
			}
		}
	}

	public static List<string> GetSources(this IEnumerable<DriveMeasurement?> measurements) {
		var sources = new List<string>();
		foreach (var measure in measurements)
		{
			if (measure != null) {
				var source = measure.States.FirstOrDefault(st => st.AttributeName == DriveAttributes.SourceAdapter);
				if (source != null) {
					sources.AddRange(source.Value.SplitSources());
				}
			}
		}
		return sources.Distinct().ToList();
	}

	internal static List<string> SplitSources(this string input) {
		return [.. input.Split(';', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Distinct()];
	}

	public static Dictionary<string, DriveMeasurement?> ToDiskDictionary(this IEnumerable<KeyValuePair<DriveEntity, DriveMeasurement?>> measurements, bool indexOnSerialNumber) {
		var dict = measurements.ToDictionary(kv => indexOnSerialNumber ? kv.Key.UniqueId.SerialNumber : kv.Key.Id, v => v.Value, StringComparer.OrdinalIgnoreCase);
		return dict;
	}

	public static string ToObjectId(this string input, char separator = '-', bool forceLowerCase = false) {
		return string.Join(separator, input.Split(" ").Select(w => forceLowerCase ? w.ToLower() : w));
	}
}