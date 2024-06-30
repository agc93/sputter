using Sputter.Core;
using System.Diagnostics.CodeAnalysis;

namespace Sputter.Synology;

[method: SetsRequiredMembers]
public class SynologyDrive(SynologyDiskInfo info) : DriveEntity(info.Serial, new UniqueId(info.Serial, info.Model)) {
	public SynologyDiskInfo DiskInfo { get; set; } = info;
}