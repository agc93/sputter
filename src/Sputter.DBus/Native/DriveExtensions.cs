namespace UDisks2.DBus
{
	static class DriveExtensions
    {
        public static Task<string> GetVendorAsync(this IDrive o) => o.GetAsync<string>("Vendor");
        public static Task<string> GetModelAsync(this IDrive o) => o.GetAsync<string>("Model");
        public static Task<string> GetRevisionAsync(this IDrive o) => o.GetAsync<string>("Revision");
        public static Task<string> GetSerialAsync(this IDrive o) => o.GetAsync<string>("Serial");
        public static Task<string> GetWWNAsync(this IDrive o) => o.GetAsync<string>("WWN");
        public static Task<string> GetIdAsync(this IDrive o) => o.GetAsync<string>("Id");
        public static Task<IDictionary<string, object>> GetConfigurationAsync(this IDrive o) => o.GetAsync<IDictionary<string, object>>("Configuration");
        public static Task<string> GetMediaAsync(this IDrive o) => o.GetAsync<string>("Media");
        public static Task<string[]> GetMediaCompatibilityAsync(this IDrive o) => o.GetAsync<string[]>("MediaCompatibility");
        public static Task<bool> GetMediaRemovableAsync(this IDrive o) => o.GetAsync<bool>("MediaRemovable");
        public static Task<bool> GetMediaAvailableAsync(this IDrive o) => o.GetAsync<bool>("MediaAvailable");
        public static Task<bool> GetMediaChangeDetectedAsync(this IDrive o) => o.GetAsync<bool>("MediaChangeDetected");
        public static Task<ulong> GetSizeAsync(this IDrive o) => o.GetAsync<ulong>("Size");
        public static Task<ulong> GetTimeDetectedAsync(this IDrive o) => o.GetAsync<ulong>("TimeDetected");
        public static Task<ulong> GetTimeMediaDetectedAsync(this IDrive o) => o.GetAsync<ulong>("TimeMediaDetected");
        public static Task<bool> GetOpticalAsync(this IDrive o) => o.GetAsync<bool>("Optical");
        public static Task<bool> GetOpticalBlankAsync(this IDrive o) => o.GetAsync<bool>("OpticalBlank");
        public static Task<uint> GetOpticalNumTracksAsync(this IDrive o) => o.GetAsync<uint>("OpticalNumTracks");
        public static Task<uint> GetOpticalNumAudioTracksAsync(this IDrive o) => o.GetAsync<uint>("OpticalNumAudioTracks");
        public static Task<uint> GetOpticalNumDataTracksAsync(this IDrive o) => o.GetAsync<uint>("OpticalNumDataTracks");
        public static Task<uint> GetOpticalNumSessionsAsync(this IDrive o) => o.GetAsync<uint>("OpticalNumSessions");
        public static Task<int> GetRotationRateAsync(this IDrive o) => o.GetAsync<int>("RotationRate");
        public static Task<string> GetConnectionBusAsync(this IDrive o) => o.GetAsync<string>("ConnectionBus");
        public static Task<string> GetSeatAsync(this IDrive o) => o.GetAsync<string>("Seat");
        public static Task<bool> GetRemovableAsync(this IDrive o) => o.GetAsync<bool>("Removable");
        public static Task<bool> GetEjectableAsync(this IDrive o) => o.GetAsync<bool>("Ejectable");
        public static Task<string> GetSortKeyAsync(this IDrive o) => o.GetAsync<string>("SortKey");
        public static Task<bool> GetCanPowerOffAsync(this IDrive o) => o.GetAsync<bool>("CanPowerOff");
        public static Task<string> GetSiblingIdAsync(this IDrive o) => o.GetAsync<string>("SiblingId");
    }
}