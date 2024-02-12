using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Partition")]
    interface IPartition : IDBusObject
    {
        Task SetTypeAsync(string Type, IDictionary<string, object> Options);
        Task SetNameAsync(string Name, IDictionary<string, object> Options);
        Task SetFlagsAsync(ulong Flags, IDictionary<string, object> Options);
        Task ResizeAsync(ulong Size, IDictionary<string, object> Options);
        Task DeleteAsync(IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<PartitionProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}