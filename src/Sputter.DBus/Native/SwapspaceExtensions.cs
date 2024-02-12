namespace UDisks2.DBus
{
	static class SwapspaceExtensions
    {
        public static Task<bool> GetActiveAsync(this ISwapspace o) => o.GetAsync<bool>("Active");
    }
}