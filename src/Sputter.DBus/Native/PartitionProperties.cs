using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class PartitionProperties
    {
        private uint _Number = default(uint);
        public uint Number
        {
            get
            {
                return _Number;
            }

            set
            {
                _Number = (value);
            }
        }

        private string _Type = default(string);
        public string Type
        {
            get
            {
                return _Type;
            }

            set
            {
                _Type = (value);
            }
        }

        private ulong _Flags = default(ulong);
        public ulong Flags
        {
            get
            {
                return _Flags;
            }

            set
            {
                _Flags = (value);
            }
        }

        private ulong _Offset = default(ulong);
        public ulong Offset
        {
            get
            {
                return _Offset;
            }

            set
            {
                _Offset = (value);
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

        private string _Name = default(string);
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = (value);
            }
        }

        private string _UUID = default(string);
        public string UUID
        {
            get
            {
                return _UUID;
            }

            set
            {
                _UUID = (value);
            }
        }

        private ObjectPath _Table = default(ObjectPath);
        public ObjectPath Table
        {
            get
            {
                return _Table;
            }

            set
            {
                _Table = (value);
            }
        }

        private bool _IsContainer = default(bool);
        public bool IsContainer
        {
            get
            {
                return _IsContainer;
            }

            set
            {
                _IsContainer = (value);
            }
        }

        private bool _IsContained = default(bool);
        public bool IsContained
        {
            get
            {
                return _IsContained;
            }

            set
            {
                _IsContained = (value);
            }
        }
    }
}