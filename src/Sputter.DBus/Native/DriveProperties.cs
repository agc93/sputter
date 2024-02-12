using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class DriveProperties
    {
        public string? Vendor { get; set; } = default;

        private string _Model = default(string);
        public string Model
        {
            get
            {
                return _Model;
            }

            set
            {
                _Model = (value);
            }
        }

        private string _Revision = default(string);
        public string Revision
        {
            get
            {
                return _Revision;
            }

            set
            {
                _Revision = (value);
            }
        }

        private string _Serial = default(string);
        public string Serial
        {
            get
            {
                return _Serial;
            }

            set
            {
                _Serial = (value);
            }
        }

        private string _WWN = default(string);
        public string WWN
        {
            get
            {
                return _WWN;
            }

            set
            {
                _WWN = (value);
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

        private IDictionary<string, object> _Configuration = default(IDictionary<string, object>);
        public IDictionary<string, object> Configuration
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

        private string _Media = default(string);
        public string Media
        {
            get
            {
                return _Media;
            }

            set
            {
                _Media = (value);
            }
        }

        private string[] _MediaCompatibility = default(string[]);
        public string[] MediaCompatibility
        {
            get
            {
                return _MediaCompatibility;
            }

            set
            {
                _MediaCompatibility = (value);
            }
        }

        private bool _MediaRemovable = default(bool);
        public bool MediaRemovable
        {
            get
            {
                return _MediaRemovable;
            }

            set
            {
                _MediaRemovable = (value);
            }
        }

        private bool _MediaAvailable = default(bool);
        public bool MediaAvailable
        {
            get
            {
                return _MediaAvailable;
            }

            set
            {
                _MediaAvailable = (value);
            }
        }

        private bool _MediaChangeDetected = default(bool);
        public bool MediaChangeDetected
        {
            get
            {
                return _MediaChangeDetected;
            }

            set
            {
                _MediaChangeDetected = (value);
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

        private ulong _TimeDetected = default(ulong);
        public ulong TimeDetected
        {
            get
            {
                return _TimeDetected;
            }

            set
            {
                _TimeDetected = (value);
            }
        }

        private ulong _TimeMediaDetected = default(ulong);
        public ulong TimeMediaDetected
        {
            get
            {
                return _TimeMediaDetected;
            }

            set
            {
                _TimeMediaDetected = (value);
            }
        }

        private bool _Optical = default(bool);
        public bool Optical
        {
            get
            {
                return _Optical;
            }

            set
            {
                _Optical = (value);
            }
        }

        private bool _OpticalBlank = default(bool);
        public bool OpticalBlank
        {
            get
            {
                return _OpticalBlank;
            }

            set
            {
                _OpticalBlank = (value);
            }
        }

        private uint _OpticalNumTracks = default(uint);
        public uint OpticalNumTracks
        {
            get
            {
                return _OpticalNumTracks;
            }

            set
            {
                _OpticalNumTracks = (value);
            }
        }

        private uint _OpticalNumAudioTracks = default(uint);
        public uint OpticalNumAudioTracks
        {
            get
            {
                return _OpticalNumAudioTracks;
            }

            set
            {
                _OpticalNumAudioTracks = (value);
            }
        }

        private uint _OpticalNumDataTracks = default(uint);
        public uint OpticalNumDataTracks
        {
            get
            {
                return _OpticalNumDataTracks;
            }

            set
            {
                _OpticalNumDataTracks = (value);
            }
        }

        private uint _OpticalNumSessions = default(uint);
        public uint OpticalNumSessions
        {
            get
            {
                return _OpticalNumSessions;
            }

            set
            {
                _OpticalNumSessions = (value);
            }
        }

        private int _RotationRate = default(int);
        public int RotationRate
        {
            get
            {
                return _RotationRate;
            }

            set
            {
                _RotationRate = (value);
            }
        }

        private string _ConnectionBus = default(string);
        public string ConnectionBus
        {
            get
            {
                return _ConnectionBus;
            }

            set
            {
                _ConnectionBus = (value);
            }
        }

        private string _Seat = default(string);
        public string Seat
        {
            get
            {
                return _Seat;
            }

            set
            {
                _Seat = (value);
            }
        }

        private bool _Removable = default(bool);
        public bool Removable
        {
            get
            {
                return _Removable;
            }

            set
            {
                _Removable = (value);
            }
        }

        private bool _Ejectable = default(bool);
        public bool Ejectable
        {
            get
            {
                return _Ejectable;
            }

            set
            {
                _Ejectable = (value);
            }
        }

        private string _SortKey = default(string);
        public string SortKey
        {
            get
            {
                return _SortKey;
            }

            set
            {
                _SortKey = (value);
            }
        }

        private bool _CanPowerOff = default(bool);
        public bool CanPowerOff
        {
            get
            {
                return _CanPowerOff;
            }

            set
            {
                _CanPowerOff = (value);
            }
        }

        private string _SiblingId = default(string);
        public string SiblingId
        {
            get
            {
                return _SiblingId;
            }

            set
            {
                _SiblingId = (value);
            }
        }
    }
}