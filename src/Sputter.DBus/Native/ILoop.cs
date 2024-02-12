using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Loop")]
    interface ILoop : IDBusObject
    {
        Task DeleteAsync(IDictionary<string, object> Options);
        Task SetAutoclearAsync(bool Value, IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<LoopProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}