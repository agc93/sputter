namespace UDisks2.DBus
{
	static class ManagerExtensions
    {
        public static Task<string> GetVersionAsync(this IManager o) => o.GetAsync<string>("Version");
        public static Task<string[]> GetSupportedFilesystemsAsync(this IManager o) => o.GetAsync<string[]>("SupportedFilesystems");
        public static Task<string[]> GetSupportedEncryptionTypesAsync(this IManager o) => o.GetAsync<string[]>("SupportedEncryptionTypes");
        public static Task<string> GetDefaultEncryptionTypeAsync(this IManager o) => o.GetAsync<string>("DefaultEncryptionType");
    }
}