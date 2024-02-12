using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class FilesystemProperties
    {
        private byte[][] _MountPoints = default(byte[][]);
        public byte[][] MountPoints
        {
            get
            {
                return _MountPoints;
            }

            set
            {
                _MountPoints = (value);
            }
        }

        private ulong _Size = default(ulong);
        public ulong Size
        {
            get
            {
                return _Size;
            }

            set
            {
                _Size = (value);
            }
        }
    }
}