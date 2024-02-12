using Microsoft.Extensions.Options;
using Sputter.Core;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Sputter.Scrutiny;

public class ScrutinyApiAdapter : IDriveSensorAdapter {
    public int Priority => 10;
    private readonly HttpClient? _client;

    public ScrutinyApiAdapter(string apiBaseAddress) {
        _client = new HttpClient() {
            BaseAddress = new Uri(apiBaseAddress)
        };
    }

    public ScrutinyApiAdapter(ScrutinyConfiguration scrutinyConfig) {

        _client = new HttpClient() {
            BaseAddress = new Uri(scrutinyConfig.ApiBaseAddress)
        };
    }

    public ScrutinyApiAdapter(IOptions<ScrutinyConfiguration> scrutinyConfig) {

        _client = new HttpClient() {
            BaseAddress = new Uri(scrutinyConfig.Value.ApiBaseAddress)
        };
    }

    public ScrutinyApiAdapter() {
        
    }
    public string Name => "scrutiny";

    private static bool MatchesFilter(ScrutinyDriveSummary driveSummary, string? filter) {
        if (filter == null) return true;
        return filter.WildcardMatch(driveSummary.Device.SerialNumber)
            || filter.WildcardMatch(driveSummary.Device.WWN)
            || ((!string.IsNullOrWhiteSpace(driveSummary.Device.ModelName)) && filter.WildcardMatch(driveSummary.Device.ModelName));
    }

    public async Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter) {
        if (_client == null) return [];
        var summary = await _client.GetFromJsonAsync("summary", SourceGenerationContext.Default.ApiResponseSummaryResponse);
        var drives = new List<DriveEntity>();
        foreach (var drive in summary?.Value?.DriveSummaries ?? []) {
            if ((!string.IsNullOrWhiteSpace(drive.Value.Device?.SerialNumber)) && MatchesFilter(drive.Value, filter)) {
                var dev = drive.Value.Device;
                var entity = new ScrutinyEntity(dev.SerialId ?? dev.WWN, new UniqueId(dev.SerialNumber, dev.ModelName)) {
                    DriveSummary = drive.Value
                };
                entity.UniqueId.WWN = dev.WWN;
                drives.Add(entity);
            }
        }
        return drives;
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
                        },
                    new DriveState {
                        AttributeName = "scrutiny",
                        Value = "first"
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
        var summary = await _client.GetFromJsonAsync("summary", SourceGenerationContext.Default.ApiResponseSummaryResponse);
        return summary?.Value.DriveSummaries.Values.ToList() ?? [];
    }
}
