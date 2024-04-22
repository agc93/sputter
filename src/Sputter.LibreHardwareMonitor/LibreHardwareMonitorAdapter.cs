using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using Microsoft.Extensions.Logging;
using Sputter.Core;
using System.Reflection;
using System.Runtime.InteropServices;
using ZiggyCreatures.Caching.Fusion;

namespace Sputter.LibreHardwareMonitor;

public class LibreHardwareMonitorAdapter : IDriveSensorAdapter {

	public const string AdapterName = "lhm";
	public static bool WarningShown { get; private set; }

	public LibreHardwareMonitorAdapter() {
		
	}

	public LibreHardwareMonitorAdapter(IFusionCacheProvider cacheProvider) {
		_cache = cacheProvider.GetCacheOrNull(AdapterName);
	}

	public LibreHardwareMonitorAdapter(ILogger<LibreHardwareMonitorAdapter> logger, IFusionCacheProvider cacheProvider) : this(cacheProvider) {
		this.logger = logger;
	}

	private readonly ILogger? logger;
	private readonly IFusionCache? _cache;

	public int Priority => 50;

	public string Name => AdapterName;

	public Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter) {
		logger?.LogDebug("Discovering drives with LHM plugin!");
		var drives = GetSensorsCached()
			.Where(sr => string.IsNullOrWhiteSpace(filter) || sr.Key.MatchesFilter(filter))
			.GroupBy(sp => sp.Key.UniqueId.SerialNumber)
			.Select(g => g.First().Key);
		return Task.FromResult(drives);
	}

	public Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true) => throw new NotImplementedException();
	public Task<DriveMeasurement?> MeasureDrive(DriveEntity drive) {
		logger?.LogDebug("Measuring drives with LHM plugin!");
		var results = GetSensorsCached();
		var match = results.FirstOrDefault(s => s.Key.UniqueId.Equals(drive.UniqueId));
		if (!match.Equals(default) && match.Value != null) {
			var measure = new DriveMeasurement(match.Key.UniqueId) {
				Sensors = [match.Value]
			};
			return Task.FromResult<DriveMeasurement?>(measure);
		}
		return Task.FromResult<DriveMeasurement?>(null);
	}

	private List<KeyValuePair<DriveEntity, DriveSensor>> GetSensorsCached() {
		return _cache == null
			? GetSensors().ToList()
			: _cache.GetOrSet($"{AdapterName}:Sensors", token => {
				return GetSensors().ToList();
			});
	}

	/// <summary>
	/// This should be faster than <see cref="GetSensors"/> but the difference is minimal (<1s) 
	/// and the slow part is deep in LHM, so this is likely to remain unused sunice we can cache 
	/// the results from <see cref="GetSensors"/> in <see cref="GetSensorsCached"/>.
	/// </summary>
	/// <returns>All discovered drives with temperature sensors available.</returns>
	private IEnumerable<DriveEntity> GetDrives() {
		var computer = GetComputer();
		computer.Open();

		foreach (var hardware in computer.Hardware.Where(hw => hw.HardwareType == HardwareType.Storage && hw.GetType().IsAssignableTo(typeof(AbstractStorage)))) {
			logger?.LogTrace("Getting sensors for {Hardware}", hardware.Name);
			var storage = (AbstractStorage)hardware;
			var _storageInfo = typeof(AbstractStorage).GetField("_storageInfo", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(storage);
			if (_storageInfo != null && _storageInfo.GetPropertyValue<string?>("Serial") is var serial && !string.IsNullOrWhiteSpace(serial)) {
				if (storage.Sensors.Any(s => s.SensorType == SensorType.Temperature)) {
					var vendor = _storageInfo.GetPropertyValue<string?>("Vendor");
					var product = _storageInfo.GetPropertyValue<string>("Product");
					var id = new UniqueId(serial, string.IsNullOrWhiteSpace(product) ? hardware.Name : product);
					var drive = new DriveEntity($"{hardware.Identifier}/{hardware.Name}", id) { Manufacturer = vendor, Capacity = storage.DriveInfos.First().TotalSize.ToString(), SoftwareVersion = storage.FirmwareRevision };
					yield return drive;
				}
			}
		}

		computer.Close();
	}

	private IEnumerable<KeyValuePair<DriveEntity, DriveSensor>> GetSensors() {
		var computer = GetComputer();

		computer.Open();
		computer.Accept(new UpdateVisitor());

		foreach (var hardware in computer.Hardware.Where(hw => hw.HardwareType == HardwareType.Storage && hw.GetType().IsAssignableTo(typeof(AbstractStorage)))) {
			logger?.LogTrace("Getting sensors for {Hardware}", hardware.Name);
			var storage = (AbstractStorage)hardware;
			var _storageInfo = typeof(AbstractStorage).GetField("_storageInfo", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(storage);
			if (_storageInfo != null && _storageInfo.GetPropertyValue<string?>("Serial") is var serial && !string.IsNullOrWhiteSpace(serial)) {
				var vendor = _storageInfo.GetPropertyValue<string?>("Vendor");
				var product = _storageInfo.GetPropertyValue<string>("Product");
				var id = new UniqueId(serial, string.IsNullOrWhiteSpace(product) ? hardware.Name : product);
				var drive = new DriveEntity($"{hardware.Identifier}/{hardware.Name}", id) { Manufacturer = vendor, Capacity = storage.DriveInfos.First().TotalSize.ToString(), SoftwareVersion = storage.FirmwareRevision };
				//foreach (IHardware subhardware in hardware.SubHardware) {
				//    logger?.LogTrace("Descending to sensors in {subhardware}", subhardware.Name);

				//    foreach (ISensor sensor in subhardware.Sensors.Where(s => s.SensorType == SensorType.Temperature)) {
				//        if (sensor.Value != null && sensor.Value > 0) {
				//            yield return new KeyValuePair<IHardware, DriveSensor>(subhardware, new DriveSensor {
				//                AttributeName = sensor.Name,
				//                Value = Convert.ToDouble(sensor.Value)
				//            });
				//        }
				//    }
				//}

				foreach (ISensor sensor in storage.Sensors.Where(s => s.SensorType == SensorType.Temperature)) {
					if (sensor.Value != null && sensor.Value > 0) {
						yield return new KeyValuePair<DriveEntity, DriveSensor>(drive, new DriveSensor {
							AttributeName = DriveAttributes.Temperature,
							Value = Convert.ToDouble(sensor.Value),
							Units = "°C" //seems like LHM reports all Temperature sensors as °C
						});
					} else {
						if (!WarningShown && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
							logger?.LogWarning("If you are not running as administrator, you won't get temperature results from the LHM plugin!");
							WarningShown = true;
						}
					}
				}
			}
		}

		computer.Close();
	}

	private static Computer GetComputer() => new() {
		IsCpuEnabled = false,
		IsGpuEnabled = false,
		IsMemoryEnabled = false,
		IsMotherboardEnabled = false,
		IsControllerEnabled = false,
		IsNetworkEnabled = false,
		IsStorageEnabled = true,

	};
}
