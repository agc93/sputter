namespace Sputter.DBus;

internal static class DbusConstants {
    internal const string UDisks2Interface = "org.freedesktop.UDisks2";
    internal const string UDisks2Path = "/org/freedesktop/UDisks2";
    internal const string UDisks2DrivesPathPrefix = UDisks2Path + "/drives/";
    internal const string AtaInterface = "org.freedesktop.UDisks2.Drive.Ata";
}
