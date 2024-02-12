using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace UDisks2.DBus
{

	static class AtaExtensions
	{
		public static Task<bool> GetSmartSupportedAsync(this IAta o) => o.GetAsync<bool>("SmartSupported");
		public static Task<bool> GetSmartEnabledAsync(this IAta o) => o.GetAsync<bool>("SmartEnabled");
		public static Task<ulong> GetSmartUpdatedAsync(this IAta o) => o.GetAsync<ulong>("SmartUpdated");
		public static Task<bool> GetSmartFailingAsync(this IAta o) => o.GetAsync<bool>("SmartFailing");
		public static Task<ulong> GetSmartPowerOnSecondsAsync(this IAta o) => o.GetAsync<ulong>("SmartPowerOnSeconds");
		public static Task<double> GetSmartTemperatureAsync(this IAta o) => o.GetAsync<double>("SmartTemperature");
		public static Task<int> GetSmartNumAttributesFailingAsync(this IAta o) => o.GetAsync<int>("SmartNumAttributesFailing");
		public static Task<int> GetSmartNumAttributesFailedInThePastAsync(this IAta o) => o.GetAsync<int>("SmartNumAttributesFailedInThePast");
		public static Task<long> GetSmartNumBadSectorsAsync(this IAta o) => o.GetAsync<long>("SmartNumBadSectors");
		public static Task<string> GetSmartSelftestStatusAsync(this IAta o) => o.GetAsync<string>("SmartSelftestStatus");
		public static Task<int> GetSmartSelftestPercentRemainingAsync(this IAta o) => o.GetAsync<int>("SmartSelftestPercentRemaining");
		public static Task<bool> GetPmSupportedAsync(this IAta o) => o.GetAsync<bool>("PmSupported");
		public static Task<bool> GetPmEnabledAsync(this IAta o) => o.GetAsync<bool>("PmEnabled");
		public static Task<bool> GetApmSupportedAsync(this IAta o) => o.GetAsync<bool>("ApmSupported");
		public static Task<bool> GetApmEnabledAsync(this IAta o) => o.GetAsync<bool>("ApmEnabled");
		public static Task<bool> GetAamSupportedAsync(this IAta o) => o.GetAsync<bool>("AamSupported");
		public static Task<bool> GetAamEnabledAsync(this IAta o) => o.GetAsync<bool>("AamEnabled");
		public static Task<int> GetAamVendorRecommendedValueAsync(this IAta o) => o.GetAsync<int>("AamVendorRecommendedValue");
		public static Task<bool> GetWriteCacheSupportedAsync(this IAta o) => o.GetAsync<bool>("WriteCacheSupported");
		public static Task<bool> GetWriteCacheEnabledAsync(this IAta o) => o.GetAsync<bool>("WriteCacheEnabled");
		public static Task<bool> GetReadLookaheadSupportedAsync(this IAta o) => o.GetAsync<bool>("ReadLookaheadSupported");
		public static Task<bool> GetReadLookaheadEnabledAsync(this IAta o) => o.GetAsync<bool>("ReadLookaheadEnabled");
		public static Task<int> GetSecurityEraseUnitMinutesAsync(this IAta o) => o.GetAsync<int>("SecurityEraseUnitMinutes");
		public static Task<int> GetSecurityEnhancedEraseUnitMinutesAsync(this IAta o) => o.GetAsync<int>("SecurityEnhancedEraseUnitMinutes");
		public static Task<bool> GetSecurityFrozenAsync(this IAta o) => o.GetAsync<bool>("SecurityFrozen");
	}
}