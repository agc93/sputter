using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Manager.NVMe")]
	interface INVMe : IDBusObject
	{
		Task SetHostNQNAsync(byte[] Hostnqn, IDictionary<string, object> Options);
		Task SetHostIDAsync(byte[] Hostid, IDictionary<string, object> Options);
		Task<ObjectPath> ConnectAsync(byte[] Subsysnqn, string Transport, string TransportAddr, IDictionary<string, object> Options);
		Task<T> GetAsync<T>(string prop);
		Task<NVMeProperties> GetAllAsync();
		Task SetAsync(string prop, object val);
		Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
	}
}