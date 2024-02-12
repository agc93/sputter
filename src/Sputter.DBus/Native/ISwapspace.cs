using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Swapspace")]
    interface ISwapspace : IDBusObject
    {
        Task StartAsync(IDictionary<string, object> Options);
        Task StopAsync(IDictionary<string, object> Options);
        Task SetLabelAsync(string Label, IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<SwapspaceProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}