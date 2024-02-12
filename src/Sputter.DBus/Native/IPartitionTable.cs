using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.PartitionTable")]
    interface IPartitionTable : IDBusObject
    {
        Task<ObjectPath> CreatePartitionAsync(ulong Offset, ulong Size, string Type, string Name, IDictionary<string, object> Options);
        Task<ObjectPath> CreatePartitionAndFormatAsync(ulong Offset, ulong Size, string Type, string Name, IDictionary<string, object> Options, string FormatType, IDictionary<string, object> FormatOptions);
        Task<T> GetAsync<T>(string prop);
        Task<PartitionTableProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}