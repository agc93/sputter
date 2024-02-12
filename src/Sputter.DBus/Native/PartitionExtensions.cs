using Tmds.DBus;

namespace UDisks2.DBus
{
	static class PartitionExtensions
    {
        public static Task<uint> GetNumberAsync(this IPartition o) => o.GetAsync<uint>("Number");
        public static Task<string> GetTypeAsync(this IPartition o) => o.GetAsync<string>("Type");
        public static Task<ulong> GetFlagsAsync(this IPartition o) => o.GetAsync<ulong>("Flags");
        public static Task<ulong> GetOffsetAsync(this IPartition o) => o.GetAsync<ulong>("Offset");
        public static Task<ulong> GetSizeAsync(this IPartition o) => o.GetAsync<ulong>("Size");
        public static Task<string> GetNameAsync(this IPartition o) => o.GetAsync<string>("Name");
        public static Task<string> GetUUIDAsync(this IPartition o) => o.GetAsync<string>("UUID");
        public static Task<ObjectPath> GetTableAsync(this IPartition o) => o.GetAsync<ObjectPath>("Table");
        public static Task<bool> GetIsContainerAsync(this IPartition o) => o.GetAsync<bool>("IsContainer");
        public static Task<bool> GetIsContainedAsync(this IPartition o) => o.GetAsync<bool>("IsContained");
    }
}