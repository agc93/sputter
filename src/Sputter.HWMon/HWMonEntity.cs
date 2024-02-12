using Sputter.Core;
using System.Diagnostics.CodeAnalysis;

namespace Sputter.HWMon {
    [method: SetsRequiredMembers]
    public class HWMonEntity(string id, UniqueId uniqueId) : DriveEntity(id, uniqueId) {
        public string? SensorPath { get; init; } = null;
    }
}
