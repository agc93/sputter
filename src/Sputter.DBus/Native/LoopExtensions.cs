using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace UDisks2.DBus
{

	static class LoopExtensions
    {
        public static Task<byte[]> GetBackingFileAsync(this ILoop o) => o.GetAsync<byte[]>("BackingFile");
        public static Task<bool> GetAutoclearAsync(this ILoop o) => o.GetAsync<bool>("Autoclear");
        public static Task<uint> GetSetupByUIDAsync(this ILoop o) => o.GetAsync<uint>("SetupByUID");
    }
}