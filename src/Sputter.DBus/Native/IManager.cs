using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Manager")]
    interface IManager : IDBusObject
    {
        Task<(bool available, string)> CanFormatAsync(string Type);
        Task<(bool available, ulong, string)> CanResizeAsync(string Type);
        Task<(bool available, string)> CanCheckAsync(string Type);
        Task<(bool available, string)> CanRepairAsync(string Type);
        Task<ObjectPath> LoopSetupAsync(CloseSafeHandle Fd, IDictionary<string, object> Options);
        Task<ObjectPath> MDRaidCreateAsync(ObjectPath[] Blocks, string Level, string Name, ulong Chunk, IDictionary<string, object> Options);
        Task EnableModulesAsync(bool Enable);
        Task EnableModuleAsync(string Name, bool Enable);
        Task<ObjectPath[]> GetBlockDevicesAsync(IDictionary<string, object> Options);
        Task<ObjectPath[]> ResolveDeviceAsync(IDictionary<string, object> Devspec, IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<ManagerProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}