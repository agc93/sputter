using Tmds.DBus;

namespace UDisks2.DBus
{
	static class PartitionTableExtensions
    {
        public static Task<ObjectPath[]> GetPartitionsAsync(this IPartitionTable o) => o.GetAsync<ObjectPath[]>("Partitions");
        public static Task<string> GetTypeAsync(this IPartitionTable o) => o.GetAsync<string>("Type");
    }
}