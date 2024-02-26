using Sputter.Core;

namespace Sputter.HWMon;
public class HWMonAdapter : IDriveSensorAdapter {
    public int Priority => 25;
    public string Name => "sysfs";
    public Task<IEnumerable<DriveEntity>> DiscoverDrives(string? filter) {
        var drives = GetDrivesWithSensors()
            .Select(GetDriveDetails)
            .Where(s => s is HWMonEntity and not null)
            .Cast<HWMonEntity>()
            .ToList();
        return Task.FromResult<IEnumerable<DriveEntity>>(drives);
    }

    public Task<DriveMeasurement?> MeasureDrive(DriveEntity drive) {
        if (drive is HWMonEntity sensorDrive && sensorDrive.SensorPath != null) {
            var measurement = GetMeasurement(sensorDrive);
            return Task.FromResult(measurement);
        } else {
            var matchingDrive = GetDrivesWithSensors().Select(GetDriveDetails).FirstOrDefault(dd => dd?.Id != null && dd.Id.Contains(drive.Id));
            var measurement = GetMeasurement(matchingDrive);
            return Task.FromResult(measurement);
        }
    }

    public Task<DriveEntity?> IdentifyDrive(string pathSpec, bool exactMatch = true) {
        if (exactMatch) {
            var sysPath = GetBlockDevicePath(pathSpec);
            if (sysPath == null) return Task.FromResult<DriveEntity?>(null);
            var drive = GetDriveDetails(sysPath);
            return Task.FromResult<DriveEntity?>(drive);
        } else {
            var drives = GetDrivesWithSensors()
                .Select(GetDriveDetails)
                .Where(s => s is HWMonEntity and not null)
                .Cast<HWMonEntity>()
                .Where(d => d.Id.Contains(pathSpec) || d.UniqueId.SerialNumber.Contains(pathSpec) || (d.UniqueId.ModelNumber != null && d.UniqueId.ModelNumber.Contains(pathSpec)))
                .FirstOrDefault();
            return Task.FromResult<DriveEntity?>(drives);
        }
    }

    private DriveMeasurement? GetMeasurement(HWMonEntity? driveEntity) {
        if (driveEntity?.SensorPath == null || !File.Exists(driveEntity.SensorPath)) return null;
        var file = File.ReadAllText(driveEntity.SensorPath).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        if (file == null || !double.TryParse(file, out var temp)) return null;
        var measure = new DriveMeasurement(driveEntity.UniqueId) {
            Sensors = [new() { AttributeName = DriveAttributes.Temperature, Units = "°C", Value = temp / 1000 }]
        };
        return measure;
    }

    private HWMonEntity? GetDriveDetails(string sysClassPath) {
        var id = GetIdForBlockDevice(sysClassPath, out var deviceProps);
        if (id?.Value != null) {
            var sensor = GetSensorPathForDrive(sysClassPath);
            var drive = new HWMonEntity(id.Value.Key, id.Value.Value) {
                SensorPath = sensor,
                SoftwareVersion = deviceProps.GetValueOrDefault(HWMonConstants.FirmwareVersion),
                Capacity = deviceProps.GetValueOrDefault(HWMonConstants.Capacity)
            };
            return drive;
        }
        return null;
    }

    private IEnumerable<string> GetDrivesWithSensors() {
        var devices = OperatingSystem.IsLinux()
            ? Directory.GetDirectories("/sys/class/nvme/", "nvme*").Where(d => Directory.GetDirectories(d, "hwmon*") is var mons && mons.Length != 0).ToList()
            : [];

        foreach (var device in devices) {
            var hwmon = Directory.GetDirectories(device, "hwmon*")!.First();
            var temps = Directory.GetFiles(hwmon, "temp*_input");
            if (temps.Length == 0) continue;
            yield return device;
        }
    }

    private static string? GetBlockDevicePath(string inputPath) {
        var ip = inputPath;
        if (ip.StartsWith("/dev/")) {
            var blockDeviceName = Directory.ResolveLinkTarget(ip, true)?.Name;
            return blockDeviceName != null && blockDeviceName.StartsWith("nvme") ? $"/sys/class/nvme/{blockDeviceName}" : null;
        } else if (ip.StartsWith("/sys/block")) {
            return $"/sys/class/nvme/{new DirectoryInfo(ip).Name}";
        } else if (ip.StartsWith("/sys/class/nvme")) {
            return ip;
        }
        return null;
    }

    private static KeyValuePair<string, UniqueId>? GetIdForBlockDevice(string sysClassPath, out Dictionary<string, string> otherProperties) {
        otherProperties = [];
        var rootDeviceName = new DirectoryInfo(sysClassPath).Name;
        var blockDeviceName = Directory.GetDirectories("/sys/block", rootDeviceName + "*").FirstOrDefault();
        if (blockDeviceName == null) return null;
        var size = System.IO.File.ReadAllLines(blockDeviceName).FirstOrDefault();
        if (size != null) {
            otherProperties.Add(HWMonConstants.Capacity, size);
        }
        var blockDeviceNumber = Directory.ResolveLinkTarget(Path.Combine(blockDeviceName, "bdi"), true)?.Name;
        var udevPath = $"/run/udev/data/b{blockDeviceNumber}";
        if (udevPath != null && File.Exists(udevPath)) {
            var details = File.ReadAllLines(udevPath).Where(l => l.StartsWith("E:")).Select(k => k.Replace("E:", string.Empty).Split('=')).Where(p => p.Length == 2);
            var dict = details.ToDictionary(k => k[0], v => v[1]);
            if (dict.TryGetValue(HWMonConstants.SerialNumber, out string? shortSerial)) {
                var unique = new UniqueId(shortSerial, HWMonConstants.Model) {
                    WWN = dict.GetValueOrDefault(HWMonConstants.WWN)
                };
                otherProperties = dict;
                return new(dict.GetValueOrDefault(HWMonConstants.DiskIdentifier) ?? shortSerial, unique);
            }
        }
        return null;
    }

    private static string? GetSensorPathForDrive(string devicePath) {
        //devicePath should be something like /sys/class/nvme/nvme0
        var hwmonPath = Directory.GetDirectories(devicePath, "hwmon*").FirstOrDefault();
        if (hwmonPath != null) {
            var temps = Directory.GetFiles(hwmonPath, "temp*_input");
            if (temps.Length == 0) return null;
            if (temps.Length == 1) {
                return temps[0];
                // temps[0] is the path to the sensor
            } else {
                var comp = temps.FirstOrDefault(t => {
                    var label = t.Replace("_input", "_label");
                    if (File.Exists(label) && File.ReadAllLines(label).FirstOrDefault() == "Composite") {
                        //gotcha
                        return true;

                    }
                    return false;
                });
                if (comp == null) {
                    return temps.First();
                } else {
                    return comp;
                }
            }
        }
        return null;
    }

    [Obsolete]
    private static Dictionary<string, string> GetDrivesWithSensorPaths(List<string>? sysClassPaths = null) {
        var sensors = new Dictionary<string, string>();

        var devices = (sysClassPaths?.ToArray() ?? Directory.GetDirectories("/sys/class/nvme/", "nvme*"))
            .Where(d => Directory.GetDirectories(d, "hwmon*") is var mons && mons.Length != 0)
            .ToList();

        foreach (var device in devices) {
            //var hwmon = Directory.GetDirectories(device, "hwmon*")!.First();
            var sensor = GetSensorPathForDrive(device);
            if (sensor != null) {
                sensors.Add(device, sensor);
            }

        }
        return sensors;

    }
}
