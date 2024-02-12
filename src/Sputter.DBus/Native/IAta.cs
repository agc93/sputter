using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Drive.Ata")]
	interface IAta : IDBusObject
	{
		Task SmartUpdateAsync(IDictionary<string, object> Options);
		Task<(byte, string, ushort, int, int, int, long, int, IDictionary<string, object>)[]> SmartGetAttributesAsync(IDictionary<string, object> Options);
		Task SmartSelftestStartAsync(string Type, IDictionary<string, object> Options);
		Task SmartSelftestAbortAsync(IDictionary<string, object> Options);
		Task SmartSetEnabledAsync(bool Value, IDictionary<string, object> Options);
		Task<byte> PmGetStateAsync(IDictionary<string, object> Options);
		Task PmStandbyAsync(IDictionary<string, object> Options);
		Task PmWakeupAsync(IDictionary<string, object> Options);
		Task SecurityEraseUnitAsync(IDictionary<string, object> Options);
		Task<T> GetAsync<T>(string prop);
		Task<AtaProperties> GetAllAsync();
		Task SetAsync(string prop, object val);
		Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
	}
}