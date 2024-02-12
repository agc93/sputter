using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class BlockProperties
    {
        private byte[] _Device = default(byte[]);
        public byte[] Device
        {
            get
            {
                return _Device;
            }

            set
            {
                _Device = (value);
            }
        }

        private byte[] _PreferredDevice = default(byte[]);
        public byte[] PreferredDevice
        {
            get
            {
                return _PreferredDevice;
            }

            set
            {
                _PreferredDevice = (value);
            }
        }

        private byte[][] _Symlinks = default(byte[][]);
        public byte[][] Symlinks
        {
            get
            {
                return _Symlinks;
            }

            set
            {
                _Symlinks = (value);
            }
        }

        private ulong _DeviceNumber = default(ulong);
        public ulong DeviceNumber
        {
            get
            {
                return _DeviceNumber;
            }

            set
            {
                _DeviceNumber = (value);
            }
        }

        private string _Id = default(string);
        public string Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = (value);
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

        private bool _ReadOnly = default(bool);
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }

            set
            {
                _ReadOnly = (value);
            }
        }

        private ObjectPath _Drive = default(ObjectPath);
        public ObjectPath Drive
        {
            get
            {
                return _Drive;
            }

            set
            {
                _Drive = (value);
            }
        }

        private ObjectPath _MDRaid = default(ObjectPath);
        public ObjectPath MDRaid
        {
            get
            {
                return _MDRaid;
            }

            set
            {
                _MDRaid = (value);
            }
        }

        private ObjectPath _MDRaidMember = default(ObjectPath);
        public ObjectPath MDRaidMember
        {
            get
            {
                return _MDRaidMember;
            }

            set
            {
                _MDRaidMember = (value);
            }
        }

        private string _IdUsage = default(string);
        public string IdUsage
        {
            get
            {
                return _IdUsage;
            }

            set
            {
                _IdUsage = (value);
            }
        }

        private string _IdType = default(string);
        public string IdType
        {
            get
            {
                return _IdType;
            }

            set
            {
                _IdType = (value);
            }
        }

        private string _IdVersion = default(string);
        public string IdVersion
        {
            get
            {
                return _IdVersion;
            }

            set
            {
                _IdVersion = (value);
            }
        }

        private string _IdLabel = default(string);
        public string IdLabel
        {
            get
            {
                return _IdLabel;
            }

            set
            {
                _IdLabel = (value);
            }
        }

        private string _IdUUID = default(string);
        public string IdUUID
        {
            get
            {
                return _IdUUID;
            }

            set
            {
                _IdUUID = (value);
            }
        }

        private (string, IDictionary<string, object>)[] _Configuration = default((string, IDictionary<string, object>)[]);
        public (string, IDictionary<string, object>)[] Configuration
        {
            get
            {
                return _Configuration;
            }

            set
            {
                _Configuration = (value);
            }
        }

        private ObjectPath _CryptoBackingDevice = default(ObjectPath);
        public ObjectPath CryptoBackingDevice
        {
            get
            {
                return _CryptoBackingDevice;
            }

            set
            {
                _CryptoBackingDevice = (value);
            }
        }

        private bool _HintPartitionable = default(bool);
        public bool HintPartitionable
        {
            get
            {
                return _HintPartitionable;
            }

            set
            {
                _HintPartitionable = (value);
            }
        }

        private bool _HintSystem = default(bool);
        public bool HintSystem
        {
            get
            {
                return _HintSystem;
            }

            set
            {
                _HintSystem = (value);
            }
        }

        private bool _HintIgnore = default(bool);
        public bool HintIgnore
        {
            get
            {
                return _HintIgnore;
            }

            set
            {
                _HintIgnore = (value);
            }
        }

        private bool _HintAuto = default(bool);
        public bool HintAuto
        {
            get
            {
                return _HintAuto;
            }

            set
            {
                _HintAuto = (value);
            }
        }

        private string _HintName = default(string);
        public string HintName
        {
            get
            {
                return _HintName;
            }

            set
            {
                _HintName = (value);
            }
        }

        private string _HintIconName = default(string);
        public string HintIconName
        {
            get
            {
                return _HintIconName;
            }

            set
            {
                _HintIconName = (value);
            }
        }

        private string _HintSymbolicIconName = default(string);
        public string HintSymbolicIconName
        {
            get
            {
                return _HintSymbolicIconName;
            }

            set
            {
                _HintSymbolicIconName = (value);
            }
        }

        private string[] _UserspaceMountOptions = default(string[]);
        public string[] UserspaceMountOptions
        {
            get
            {
                return _UserspaceMountOptions;
            }

            set
            {
                _UserspaceMountOptions = (value);
            }
        }
    }
}