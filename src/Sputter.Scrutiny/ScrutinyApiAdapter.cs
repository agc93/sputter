using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sputter.Core;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ZiggyCreatures.Caching.Fusion;

namespace Sputter.Scrutiny;

public class ScrutinyApiAdapter : IDriveSensorAdapter {
    public static string AdapterName => "scrutiny";
    public int Priority => 10;
    private readonly HttpClient? _client;
    private readonly ILogger<ScrutinyApiAdapter>? _logger;
    private readonly IFusionCache? _cache;
    private const string _cacheKey = "scrutiny:summary";

    public ScrutinyApiAdapter(string apiBaseAddress) {
        _client = new HttpClient() {
            BaseAddress = new Uri(apiBaseAddress)
        };
    }

    private ScrutinyApiAdapter(IFusionCacheProvider? cacheProvider, ILogger<ScrutinyApiAdapter>? logger) {
        _cache = cacheProvider?.GetCacheOrNull(Name);
        _logger = logger;
    }

    public ScrutinyApiAdapter(ScrutinyConfiguration scrutinyConfig, IFusionCacheProvider? cache, ILogger<ScrutinyApiAdapter>? logger) : this(cache, logger) {
        _client = new HttpClient() {
            BaseAddress = new Uri(scrutinyConfig.ApiBaseAddress)
        };
    }

    public ScrutinyApiAdapter(IOptions<ScrutinyConfiguration> scrutinyConfig, IFusionCacheProvider? cache, ILogger<ScrutinyApiAdapter>? logger) : this(cache, logger) {
        if (!string.IsNullOrWhiteSpace(scrutinyConfig.Value.ApiBaseAddress) && Uri.TryCreate(scrutinyConfig.Value.ApiBaseAddress, UriKind.Absolute, out var _)) {
            _client = new HttpClient() {
                BaseAddress = new Uri(scrutinyConfig.Value.ApiBaseAddress)
            };
        } else {
            _logger?.LogDebug("No API address configured, skipping Scrutiny adapter setup.");
        }
    }

    //public ScrutinyApiAdapter() {
        
    //}
    public string Name => AdapterName;

    private static bool MatchesFilter(ScrutinyDriveSummary driveSummary, string? filter) {
        if (filter == null) return true;
        return filter.WildcardMatch(driveSummary.Device.SerialNumber)
            || filter.WildcardMatch(driveSummary.Device.WWN)
            || ((!string.IsNullOrWhiteSpace(driveSummary.Device.ModelName)) && filter.WildcardMatch(driveSummary.Device.ModelName));
    }

    public async Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter) {
        if (_client == null) return [];
        //ApiResponse<SummaryResponse>? summary = await GetDriveSummary(_client);
        var drives = new List<DriveEntity>();
        var summaries = await GetSummaries();
        foreach (var drive in summaries) {
            if ((!string.IsNullOrWhiteSpace(drive.Device?.SerialNumber)) && MatchesFilter(drive, filter)) {
                var dev = drive.Device;
                var entity = new ScrutinyEntity(GetDeviceId(dev), new UniqueId(dev.SerialNumber, dev.ModelName)) {
                    DriveSummary = drive
                };
                entity.UniqueId.WWN = dev.WWN;
                drives.Add(entity);
            }
        }
        return drives;

        string GetDeviceId(ScrutinyDeviceDetails dev) {
	        return string.IsNullOrWhiteSpace(dev.SerialId)
		        ? string.IsNullOrWhiteSpace(dev.WWN)
			        ? dev.SerialNumber
			        : dev.WWN
		        : dev.SerialId;
        }
    }

    public Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true) {
        throw new NotImplementedException();
    }

    private static T AddDriveInfo<T>(T drive, ScrutinyDriveSummary summary) where T : DriveEntity {
        try {
            drive.Capacity = summary.Device.Size.ToString();
            drive.SoftwareVersion = summary.Device.Firmware;
        } catch {
            //ignored
        }
        return drive;
    }

    public async Task<DriveMeasurement?> MeasureDrive(DriveEntity drive) {
        if (_client == null) return null;
        if (drive is ScrutinyEntity scrutinyDrive && scrutinyDrive is { DriveSummary: { } }) {
            scrutinyDrive = AddDriveInfo(scrutinyDrive, scrutinyDrive.DriveSummary);
            //reusing
            return new DriveMeasurement(drive.UniqueId) {
                Sensors = [
                        new DriveSensor {
                            AttributeName = DriveAttributes.Temperature,
                            Units = "°C",
                            Value = scrutinyDrive.DriveSummary!.SmartInfo.Temperature
                        }
                    ],
                States = [
                        new DriveState {
                            AttributeName = DriveAttributes.Healthy,
                            Value = (scrutinyDrive.DriveSummary.Device.DeviceStatus == 0).ToString()
                        }
                    ]
            };
        } else {
            var allDrives = await GetSummaries();
            var matching = allDrives.FirstOrDefault(d => d.Device.SerialNumber == drive.UniqueId.SerialNumber);
            if (matching != null) {
                drive = AddDriveInfo(drive, matching);
                return new DriveMeasurement(drive.UniqueId) {
                    Sensors = [
                        new DriveSensor {
                            AttributeName = DriveAttributes.Temperature,
                            Units = "°C",
                            Value = matching.SmartInfo.Temperature
                        }
                    ],
                    States = [
                        new DriveState {
                            AttributeName = DriveAttributes.Healthy,
                            Value = (matching.Device.DeviceStatus == 0).ToString()
                        }
                    ]
                };
            }
        }
        return null;
    }

    private async Task<IEnumerable<ScrutinyDriveSummary>> GetSummaries() {
        if (_client == null) throw new ArgumentNullException(nameof(_client));
        //var summary = await _client.GetFromJsonAsync("summary", SourceGenerationContext.Default.ApiResponseSummaryResponse);
        var summary = _cache == null
                ? await _client.GetFromJsonAsync("summary", SourceGenerationContext.Default.ApiResponseSummaryResponse)
                : await _cache.GetOrSetAsync(_cacheKey, async token => {
                    return await _client.GetFromJsonAsync("summary", SourceGenerationContext.Default.ApiResponseSummaryResponse, token);
                });
        return summary?.Value.DriveSummaries.Values.ToList() ?? [];
    }
}
