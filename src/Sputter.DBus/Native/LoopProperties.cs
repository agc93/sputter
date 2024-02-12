using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class LoopProperties
    {
        private byte[] _BackingFile = default(byte[]);
        public byte[] BackingFile
        {
            get
            {
                return _BackingFile;
            }

            set
            {
                _BackingFile = (value);
            }
        }

        private bool _Autoclear = default(bool);
        public bool Autoclear
        {
            get
            {
                return _Autoclear;
            }

            set
            {
                _Autoclear = (value);
            }
        }

        private uint _SetupByUID = default(uint);
        public uint SetupByUID
        {
            get
            {
                return _SetupByUID;
            }

            set
            {
                _SetupByUID = (value);
            }
        }
    }
}