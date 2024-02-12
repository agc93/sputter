using Tmds.DBus;

namespace UDisks2.DBus
{
	static class EncryptedExtensions
    {
        public static Task<(string, IDictionary<string, object>)[]> GetChildConfigurationAsync(this IEncrypted o) => o.GetAsync<(string, IDictionary<string, object>)[]>("ChildConfiguration");
        public static Task<string> GetHintEncryptionTypeAsync(this IEncrypted o) => o.GetAsync<string>("HintEncryptionType");
        public static Task<ulong> GetMetadataSizeAsync(this IEncrypted o) => o.GetAsync<ulong>("MetadataSize");
        public static Task<ObjectPath> GetCleartextDeviceAsync(this IEncrypted o) => o.GetAsync<ObjectPath>("CleartextDevice");
    }
}