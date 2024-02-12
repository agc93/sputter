using Sputter.Core;
using System.Diagnostics.CodeAnalysis;

namespace Sputter.Scrutiny;

[method: SetsRequiredMembers]
public class ScrutinyEntity(string id, UniqueId uniqueId) : DriveEntity(id, uniqueId) {
    public ScrutinyDriveSummary? DriveSummary { get; init; } = null;
}
