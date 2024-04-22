using Sputter.Core;
using Tmds.DBus;
using UDisks2.DBus;
using static Sputter.DBus.DbusConstants;

namespace Sputter.DBus;

public class DBusAdapter : IDriveSensorAdapter {
	public int Priority => 20;
	public string Name => "dbus";
	public async Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter) {

		var supported = await GetDriveObjectsAsync(Connection.System, filter) ?? [];
		var entities = new List<DriveEntity>();
		foreach (var item in supported) {
			var id = await item.Value.ToId();
			entities.Add(new DBusEntity(item.Key, id) { Drive = item.Value });
		}
		return entities;
	}

	public async Task<DriveMeasurement?> MeasureDrive(DriveEntity drive) {
		var conn = Connection.System;
		//var paths = await GetDrivePaths(conn);
		//var matching = paths.FirstOrDefault(p => p.Contains(drive.UniqueId.SerialNumber, StringComparison.InvariantCultureIgnoreCase));
		var dbus = (drive is DBusEntity dbusDrive && dbusDrive is { Drive: { } })
			? dbusDrive.Drive
			: await GetDrive(conn, drive.Id);
		if (dbus != null) {
			var id = drive.UniqueId;
			//var id = await dbus.ToId();
			var ataProps = await dbus.Ata.GetAllAsync();
			var props = await dbus.Drive.GetAllAsync();
			if (props != null) {
				try {
					drive.SoftwareVersion = props.Revision;
					drive.Manufacturer = props.Vendor;
					drive.Capacity = props.Size.ToString();
				} catch {
					//ignored
				}
			}
			var temp = ataProps.SmartTemperature - 273.15;
			return new DriveMeasurement(id) {
				Sensors = [
					new DriveSensor { AttributeName = DriveAttributes.Temperature, Value = temp, Units = "°C" }
				],
				States = [
					new DriveState { AttributeName = DriveAttributes.Healthy, Value = (!ataProps.SmartFailing).ToString() }
				]
			};
		}
		return null;
	}

	public async Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true) {
		if (exactMatch) {
			pathSpec = pathSpec.StartsWith(UDisks2DrivesPathPrefix) ? pathSpec : UDisks2DrivesPathPrefix + pathSpec;
			var dbusDrv = await GetDrive(Connection.System, pathSpec);
			if (dbusDrv == null) {
				return null;
			}
			var id = await dbusDrv.ToId();
			return new DBusEntity(pathSpec, id) { Drive = dbusDrv };
		} else {
			pathSpec = pathSpec.StartsWith(UDisks2DrivesPathPrefix) ? pathSpec : $"{UDisks2DrivesPathPrefix}*{pathSpec.TrimStart('*')}";
			var supported = await GetDriveObjectsAsync(Connection.System, pathSpec) ?? [];
			var entities = new List<DriveEntity>();
			foreach (var item in supported) {
				var id = await item.Value.ToId();
				entities.Add(new DBusEntity(item.Key, id) { Drive = item.Value });
			}
			return entities.First();
		}
	}

	private static async Task<DBusDrive?> GetDrive(Connection conn, string objectPath) {
		try {
			var ata = conn.CreateProxy<IAta>(UDisks2Interface, objectPath);
			var drv = conn.CreateProxy<IDrive>(UDisks2Interface, objectPath);
			var aall = await ata.GetAllAsync();
			if (aall is { SmartEnabled: true }) {
				return new DBusDrive(drv, ata);
			}
			return null;
		} catch {
			return null;
		}

	}

	private static async Task<List<string>> GetDrivePaths(Connection conn) {
		var mgr = conn.CreateProxy<IObjectManager>(UDisks2Interface, UDisks2Path);
		var objs = await mgr.GetManagedObjectsAsync();
		var supported = objs.Where(o => o.Key.ToString().StartsWith(UDisks2DrivesPathPrefix) && o.Value.ContainsKey(AtaInterface)).ToList();
		return supported.Select(s => s.Key.ToString()).ToList();
	}

	private static async Task<Dictionary<string, DBusDrive>> GetDriveObjectsAsync(Connection conn, string? filter = null) {
		var objs = await GetManagedObjectsAsync(conn);
		if (objs == null) return [];
		var supported = objs.Where(o => o.Key.ToString().StartsWith(UDisks2DrivesPathPrefix) && o.Value.ContainsKey(AtaInterface)).ToList();
		var matched = string.IsNullOrWhiteSpace(filter) ? supported : supported.Where(s => filter.WildcardMatch(s.Key.ToString()));
		var dict = new Dictionary<string, DBusDrive>();
		foreach (var drive in matched) {
			try {
				var dbusDrv = await GetDrive(conn, drive.Key.ToString());
				if (dbusDrv != null) {
					dict.Add(drive.Key.ToString(), dbusDrv);
				}
			} catch {
				//ignored
			}
		}
		return dict;
		//return supported.Select(s => s.Key.ToString()).ToList();
	}

	private static async Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>?> GetManagedObjectsAsync(Connection conn) {
		try {
			var mgr = conn.CreateProxy<IObjectManager>(UDisks2Interface, UDisks2Path);
			var objs = await mgr.GetManagedObjectsAsync();
			return objs;
		} catch {
			return null;
		}
	}
}
