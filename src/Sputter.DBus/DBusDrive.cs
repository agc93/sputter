using UDisks2.DBus;

namespace Sputter.DBus;

internal class DBusDrive(IDrive drive, IAta ata) {
    internal IDrive Drive { get; set; } = drive;
    internal IAta Ata { get; set; } = ata;
}
