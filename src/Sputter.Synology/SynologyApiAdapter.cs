using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sputter.Core;
using ZiggyCreatures.Caching.Fusion;

namespace Sputter.Synology;

public class SynologyApiAdapter : IDriveSensorAdapter {
	public int Priority => 50;
	
	public const string AdapterName = "synology-api";
	public string Name => AdapterName;
	
	private readonly ILogger? _logger;
	private readonly IFusionCache? _cache;
	private readonly IOptions<List<SynologyConfiguration>>? _options;
	private readonly IConfiguration? _config;

	private Dictionary<string, SynologyApiClient> ApiClients { get; set; } = [];

	public SynologyApiAdapter() {
		
	}

	public SynologyApiAdapter(ILogger<SynologyApiAdapter> logger, IFusionCacheProvider cacheProvider) : this() {
		_cache = cacheProvider.GetCacheOrNull(AdapterName);
		_logger = logger;
	}

	public SynologyApiAdapter(IOptions<List<SynologyConfiguration>> options, ILogger<SynologyApiAdapter> logger, IFusionCacheProvider cacheProvider) : this(logger,
		cacheProvider) {
		_options = options;
	}
	
	// public SynologyApiAdapter(IConfiguration config, ILogger<SynologyApiAdapter> logger, IFusionCacheProvider cacheProvider) : this(logger,
	// 	cacheProvider) {
	// 	_config = config;
	// }
	
	private static bool MatchesFilter(SynologyDiskInfo driveSummary, string? filter) {
		if (filter == null) return true;
		return filter.WildcardMatch(driveSummary.Device)
		       || filter.WildcardMatch(driveSummary.Model)
		       || filter.WildcardMatch(driveSummary.Serial)
		       || filter.WildcardMatch(driveSummary.Name);
	}
	
	public async Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter) {
		_logger?.LogDebug("Discovering drives with Synology plugin!");
		if (_options == null && _config == null) return [];
		var configs = GetConfigs();
		foreach (var config in configs.Where(c => c is {Host: not null})) {
			var client = ApiClients.GetValueOrDefault(config.Host!) ?? await GetClient(config);
			if (client != null) {
				ApiClients[config.Host!] = client;
				var disks = (await client.GetDisks()).ToList();
				var drives = disks
					.Where(sr => string.IsNullOrWhiteSpace(filter) || MatchesFilter(sr, filter))
					.ToList();
				return drives.Select(d => new SynologyDrive(d));
			}
		}
		return [];
	}

	private async Task<SynologyApiClient?> GetClient(SynologyConfiguration config) {
		if (_options == null && _config == null) return null;
		if (config.Host == null || config.User == null || config.Password == null) return null;
		var client = new SynologyApiClient(config);
		var resp = await client.Initialize();
		return resp ? client : null;
	}

	private List<SynologyConfiguration> GetConfigs() {
		if (_options == null && _config == null) throw new ArgumentNullException(nameof(_options));
		return _options == null
			? _config!.GetSection("Synology").Get<List<SynologyConfiguration>>() ?? throw new InvalidOperationException()
			: _options.Value;
	}
	
	private static T AddDriveInfo<T>(T drive, SynologyDiskInfo summary) where T : DriveEntity {
		try {
			drive.Capacity = summary.SizeBytes.ToString();
			drive.SoftwareVersion = summary.Firmware;
			drive.Manufacturer = summary.Vendor.Trim();
		} catch {
			//ignored
		}
		return drive;
	}

	public async Task<DriveMeasurement?> MeasureDrive(DriveEntity drive) {
		if (_options == null && _config == null) return null;
		if (drive is SynologyDrive { DiskInfo: not null } synDrive) {
			synDrive = AddDriveInfo(synDrive, synDrive.DiskInfo);
			//reusing
			return BuildMeasurement(synDrive);
		} else {
			var allDrives = await DiscoverDrives(null);
			if (allDrives.FirstOrDefault(d => d.UniqueId.SerialNumber == drive.UniqueId.SerialNumber) is SynologyDrive matching) {
				matching = AddDriveInfo(matching, matching.DiskInfo);
				return BuildMeasurement(matching);
			}
		}
		return null;

		DriveMeasurement BuildMeasurement(SynologyDrive currentDrive) {
			return new DriveMeasurement(currentDrive.UniqueId) {
				Sensors = [
					new DriveSensor {
						AttributeName = DriveAttributes.Temperature,
						Units = "°C",
						Value = currentDrive.DiskInfo.Temperature
					}
				],
				States = [
					new DriveState {
						AttributeName = DriveAttributes.Healthy,
						Value = string.Equals(currentDrive.DiskInfo.Status, "normal", StringComparison.InvariantCultureIgnoreCase).ToString()
					}
				]
			};
		}
	}

	public async Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true) => throw new NotImplementedException();
}