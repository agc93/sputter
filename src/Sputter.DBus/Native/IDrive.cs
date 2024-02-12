using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Drive")]
    interface IDrive : IDBusObject
    {
        Task EjectAsync(IDictionary<string, object> Options);
        Task SetConfigurationAsync(IDictionary<string, object> Value, IDictionary<string, object> Options);
        Task PowerOffAsync(IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<DriveProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}