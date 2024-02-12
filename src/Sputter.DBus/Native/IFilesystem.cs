using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Filesystem")]
    interface IFilesystem : IDBusObject
    {
        Task SetLabelAsync(string Label, IDictionary<string, object> Options);
        Task<string> MountAsync(IDictionary<string, object> Options);
        Task UnmountAsync(IDictionary<string, object> Options);
        Task ResizeAsync(ulong Size, IDictionary<string, object> Options);
        Task<bool> CheckAsync(IDictionary<string, object> Options);
        Task<bool> RepairAsync(IDictionary<string, object> Options);
        Task TakeOwnershipAsync(IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<FilesystemProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}