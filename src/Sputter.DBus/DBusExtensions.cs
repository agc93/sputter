using Sputter.Core;

namespace Sputter.DBus;

internal static class DBusExtensions {
    internal static async Task<UniqueId> ToId(this DBusDrive drive) {
        var props = await drive.Drive.GetAllAsync();
        return new UniqueId(props.Serial, props.Model) {
            WWN = props.WWN
        };
    }
}
