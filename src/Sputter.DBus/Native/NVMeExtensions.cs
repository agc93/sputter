namespace UDisks2.DBus
{
	static class NVMeExtensions
	{
		public static Task<byte[]> GetHostNQNAsync(this INVMe o) => o.GetAsync<byte[]>("HostNQN");
		public static Task<byte[]> GetHostIDAsync(this INVMe o) => o.GetAsync<byte[]>("HostID");
	}
}