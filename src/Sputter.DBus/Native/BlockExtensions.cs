using Tmds.DBus;

namespace UDisks2.DBus
{
	static class BlockExtensions
    {
        public static Task<byte[]> GetDeviceAsync(this IBlock o) => o.GetAsync<byte[]>("Device");
        public static Task<byte[]> GetPreferredDeviceAsync(this IBlock o) => o.GetAsync<byte[]>("PreferredDevice");
        public static Task<byte[][]> GetSymlinksAsync(this IBlock o) => o.GetAsync<byte[][]>("Symlinks");
        public static Task<ulong> GetDeviceNumberAsync(this IBlock o) => o.GetAsync<ulong>("DeviceNumber");
        public static Task<string> GetIdAsync(this IBlock o) => o.GetAsync<string>("Id");
        public static Task<ulong> GetSizeAsync(this IBlock o) => o.GetAsync<ulong>("Size");
        public static Task<bool> GetReadOnlyAsync(this IBlock o) => o.GetAsync<bool>("ReadOnly");
        public static Task<ObjectPath> GetDriveAsync(this IBlock o) => o.GetAsync<ObjectPath>("Drive");
        public static Task<ObjectPath> GetMDRaidAsync(this IBlock o) => o.GetAsync<ObjectPath>("MDRaid");
        public static Task<ObjectPath> GetMDRaidMemberAsync(this IBlock o) => o.GetAsync<ObjectPath>("MDRaidMember");
        public static Task<string> GetIdUsageAsync(this IBlock o) => o.GetAsync<string>("IdUsage");
        public static Task<string> GetIdTypeAsync(this IBlock o) => o.GetAsync<string>("IdType");
        public static Task<string> GetIdVersionAsync(this IBlock o) => o.GetAsync<string>("IdVersion");
        public static Task<string> GetIdLabelAsync(this IBlock o) => o.GetAsync<string>("IdLabel");
        public static Task<string> GetIdUUIDAsync(this IBlock o) => o.GetAsync<string>("IdUUID");
        public static Task<(string, IDictionary<string, object>)[]> GetConfigurationAsync(this IBlock o) => o.GetAsync<(string, IDictionary<string, object>)[]>("Configuration");
        public static Task<ObjectPath> GetCryptoBackingDeviceAsync(this IBlock o) => o.GetAsync<ObjectPath>("CryptoBackingDevice");
        public static Task<bool> GetHintPartitionableAsync(this IBlock o) => o.GetAsync<bool>("HintPartitionable");
        public static Task<bool> GetHintSystemAsync(this IBlock o) => o.GetAsync<bool>("HintSystem");
        public static Task<bool> GetHintIgnoreAsync(this IBlock o) => o.GetAsync<bool>("HintIgnore");
        public static Task<bool> GetHintAutoAsync(this IBlock o) => o.GetAsync<bool>("HintAuto");
        public static Task<string> GetHintNameAsync(this IBlock o) => o.GetAsync<string>("HintName");
        public static Task<string> GetHintIconNameAsync(this IBlock o) => o.GetAsync<string>("HintIconName");
        public static Task<string> GetHintSymbolicIconNameAsync(this IBlock o) => o.GetAsync<string>("HintSymbolicIconName");
        public static Task<string[]> GetUserspaceMountOptionsAsync(this IBlock o) => o.GetAsync<string[]>("UserspaceMountOptions");
    }
}