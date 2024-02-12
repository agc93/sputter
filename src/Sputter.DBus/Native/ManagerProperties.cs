using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class ManagerProperties
    {
        private string _Version = default(string);
        public string Version
        {
            get
            {
                return _Version;
            }

            set
            {
                _Version = (value);
            }
        }

        private string[] _SupportedFilesystems = default(string[]);
        public string[] SupportedFilesystems
        {
            get
            {
                return _SupportedFilesystems;
            }

            set
            {
                _SupportedFilesystems = (value);
            }
        }

        private string[] _SupportedEncryptionTypes = default(string[]);
        public string[] SupportedEncryptionTypes
        {
            get
            {
                return _SupportedEncryptionTypes;
            }

            set
            {
                _SupportedEncryptionTypes = (value);
            }
        }

        private string _DefaultEncryptionType = default(string);
        public string DefaultEncryptionType
        {
            get
            {
                return _DefaultEncryptionType;
            }

            set
            {
                _DefaultEncryptionType = (value);
            }
        }
    }
}