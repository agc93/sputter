namespace UDisks2.DBus
{
	static class FilesystemExtensions
    {
        public static Task<byte[][]> GetMountPointsAsync(this IFilesystem o) => o.GetAsync<byte[][]>("MountPoints");
        public static Task<ulong> GetSizeAsync(this IFilesystem o) => o.GetAsync<ulong>("Size");
    }
}