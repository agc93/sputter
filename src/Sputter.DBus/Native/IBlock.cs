using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Block")]
    interface IBlock : IDBusObject
    {
        Task AddConfigurationItemAsync((string, IDictionary<string, object>) Item, IDictionary<string, object> Options);
        Task RemoveConfigurationItemAsync((string, IDictionary<string, object>) Item, IDictionary<string, object> Options);
        Task UpdateConfigurationItemAsync((string, IDictionary<string, object>) OldItem, (string, IDictionary<string, object>) NewItem, IDictionary<string, object> Options);
        Task<(string, IDictionary<string, object>)[]> GetSecretConfigurationAsync(IDictionary<string, object> Options);
        Task FormatAsync(string Type, IDictionary<string, object> Options);
        Task<CloseSafeHandle> OpenForBackupAsync(IDictionary<string, object> Options);
        Task<CloseSafeHandle> OpenForRestoreAsync(IDictionary<string, object> Options);
        Task<CloseSafeHandle> OpenForBenchmarkAsync(IDictionary<string, object> Options);
        Task<CloseSafeHandle> OpenDeviceAsync(string Mode, IDictionary<string, object> Options);
        Task RescanAsync(IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<BlockProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}