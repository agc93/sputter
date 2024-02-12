using Tmds.DBus;

namespace UDisks2.DBus
{
	[DBusInterface("org.freedesktop.UDisks2.Encrypted")]
    interface IEncrypted : IDBusObject
    {
        Task<ObjectPath> UnlockAsync(string Passphrase, IDictionary<string, object> Options);
        Task LockAsync(IDictionary<string, object> Options);
        Task ChangePassphraseAsync(string Passphrase, string NewPassphrase, IDictionary<string, object> Options);
        Task ResizeAsync(ulong Size, IDictionary<string, object> Options);
        Task<T> GetAsync<T>(string prop);
        Task<EncryptedProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }
}