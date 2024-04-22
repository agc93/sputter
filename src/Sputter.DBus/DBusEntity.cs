using Sputter.Core;
using System.Diagnostics.CodeAnalysis;

namespace Sputter.DBus;

[method: SetsRequiredMembers]
public class DBusEntity(string id, UniqueId uniqueId) : DriveEntity(id, uniqueId) {
	internal DBusDrive? Drive { get; init; } = null;
}
