using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
	class NVMeProperties
	{
		private byte[] _HostNQN = default(byte[]);
		public byte[] HostNQN
		{
			get
			{
				return _HostNQN;
			}

			set
			{
				_HostNQN = (value);
			}
		}

		private byte[] _HostID = default(byte[]);
		public byte[] HostID
		{
			get
			{
				return _HostID;
			}

			set
			{
				_HostID = (value);
			}
		}
	}
}