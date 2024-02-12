using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class EncryptedProperties
    {
        private (string, IDictionary<string, object>)[] _ChildConfiguration = default((string, IDictionary<string, object>)[]);
        public (string, IDictionary<string, object>)[] ChildConfiguration
        {
            get
            {
                return _ChildConfiguration;
            }

            set
            {
                _ChildConfiguration = (value);
            }
        }

        private string _HintEncryptionType = default(string);
        public string HintEncryptionType
        {
            get
            {
                return _HintEncryptionType;
            }

            set
            {
                _HintEncryptionType = (value);
            }
        }

        private ulong _MetadataSize = default(ulong);
        public ulong MetadataSize
        {
            get
            {
                return _MetadataSize;
            }

            set
            {
                _MetadataSize = (value);
            }
        }

        private ObjectPath _CleartextDevice = default(ObjectPath);
        public ObjectPath CleartextDevice
        {
            get
            {
                return _CleartextDevice;
            }

            set
            {
                _CleartextDevice = (value);
            }
        }
    }
}