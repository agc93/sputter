using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class PartitionTableProperties
    {
        private ObjectPath[] _Partitions = default(ObjectPath[]);
        public ObjectPath[] Partitions
        {
            get
            {
                return _Partitions;
            }

            set
            {
                _Partitions = (value);
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
    }
}