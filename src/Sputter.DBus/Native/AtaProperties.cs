using Tmds.DBus;

#nullable disable

namespace UDisks2.DBus
{
	[Dictionary]
	class AtaProperties
	{
		private bool _SmartSupported = default(bool);
		public bool SmartSupported
		{
			get
			{
				return _SmartSupported;
			}

			set
			{
				_SmartSupported = (value);
			}
		}

		private bool _SmartEnabled = default(bool);
		public bool SmartEnabled
		{
			get
			{
				return _SmartEnabled;
			}

			set
			{
				_SmartEnabled = (value);
			}
		}

		private ulong _SmartUpdated = default(ulong);
		public ulong SmartUpdated
		{
			get
			{
				return _SmartUpdated;
			}

			set
			{
				_SmartUpdated = (value);
			}
		}

		private bool _SmartFailing = default(bool);
		public bool SmartFailing
		{
			get
			{
				return _SmartFailing;
			}

			set
			{
				_SmartFailing = (value);
			}
		}

		private ulong _SmartPowerOnSeconds = default(ulong);
		public ulong SmartPowerOnSeconds
		{
			get
			{
				return _SmartPowerOnSeconds;
			}

			set
			{
				_SmartPowerOnSeconds = (value);
			}
		}

		private double _SmartTemperature = default(double);
		public double SmartTemperature
		{
			get
			{
				return _SmartTemperature;
			}

			set
			{
				_SmartTemperature = (value);
			}
		}

		private int _SmartNumAttributesFailing = default(int);
		public int SmartNumAttributesFailing
		{
			get
			{
				return _SmartNumAttributesFailing;
			}

			set
			{
				_SmartNumAttributesFailing = (value);
			}
		}

		private int _SmartNumAttributesFailedInThePast = default(int);
		public int SmartNumAttributesFailedInThePast
		{
			get
			{
				return _SmartNumAttributesFailedInThePast;
			}

			set
			{
				_SmartNumAttributesFailedInThePast = (value);
			}
		}

		private long _SmartNumBadSectors = default(long);
		public long SmartNumBadSectors
		{
			get
			{
				return _SmartNumBadSectors;
			}

			set
			{
				_SmartNumBadSectors = (value);
			}
		}

		private string _SmartSelftestStatus = default(string);
		public string SmartSelftestStatus
		{
			get
			{
				return _SmartSelftestStatus;
			}

			set
			{
				_SmartSelftestStatus = (value);
			}
		}

		private int _SmartSelftestPercentRemaining = default(int);
		public int SmartSelftestPercentRemaining
		{
			get
			{
				return _SmartSelftestPercentRemaining;
			}

			set
			{
				_SmartSelftestPercentRemaining = (value);
			}
		}

		private bool _PmSupported = default(bool);
		public bool PmSupported
		{
			get
			{
				return _PmSupported;
			}

			set
			{
				_PmSupported = (value);
			}
		}

		private bool _PmEnabled = default(bool);
		public bool PmEnabled
		{
			get
			{
				return _PmEnabled;
			}

			set
			{
				_PmEnabled = (value);
			}
		}

		private bool _ApmSupported = default(bool);
		public bool ApmSupported
		{
			get
			{
				return _ApmSupported;
			}

			set
			{
				_ApmSupported = (value);
			}
		}

		private bool _ApmEnabled = default(bool);
		public bool ApmEnabled
		{
			get
			{
				return _ApmEnabled;
			}

			set
			{
				_ApmEnabled = (value);
			}
		}

		private bool _AamSupported = default(bool);
		public bool AamSupported
		{
			get
			{
				return _AamSupported;
			}

			set
			{
				_AamSupported = (value);
			}
		}

		private bool _AamEnabled = default(bool);
		public bool AamEnabled
		{
			get
			{
				return _AamEnabled;
			}

			set
			{
				_AamEnabled = (value);
			}
		}

		private int _AamVendorRecommendedValue = default(int);
		public int AamVendorRecommendedValue
		{
			get
			{
				return _AamVendorRecommendedValue;
			}

			set
			{
				_AamVendorRecommendedValue = (value);
			}
		}

		private bool _WriteCacheSupported = default(bool);
		public bool WriteCacheSupported
		{
			get
			{
				return _WriteCacheSupported;
			}

			set
			{
				_WriteCacheSupported = (value);
			}
		}

		private bool _WriteCacheEnabled = default(bool);
		public bool WriteCacheEnabled
		{
			get
			{
				return _WriteCacheEnabled;
			}

			set
			{
				_WriteCacheEnabled = (value);
			}
		}

		private bool _ReadLookaheadSupported = default(bool);
		public bool ReadLookaheadSupported
		{
			get
			{
				return _ReadLookaheadSupported;
			}

			set
			{
				_ReadLookaheadSupported = (value);
			}
		}

		private bool _ReadLookaheadEnabled = default(bool);
		public bool ReadLookaheadEnabled
		{
			get
			{
				return _ReadLookaheadEnabled;
			}

			set
			{
				_ReadLookaheadEnabled = (value);
			}
		}

		private int _SecurityEraseUnitMinutes = default(int);
		public int SecurityEraseUnitMinutes
		{
			get
			{
				return _SecurityEraseUnitMinutes;
			}

			set
			{
				_SecurityEraseUnitMinutes = (value);
			}
		}

		private int _SecurityEnhancedEraseUnitMinutes = default(int);
		public int SecurityEnhancedEraseUnitMinutes
		{
			get
			{
				return _SecurityEnhancedEraseUnitMinutes;
			}

			set
			{
				_SecurityEnhancedEraseUnitMinutes = (value);
			}
		}

		private bool _SecurityFrozen = default(bool);
		public bool SecurityFrozen
		{
			get
			{
				return _SecurityFrozen;
			}

			set
			{
				_SecurityFrozen = (value);
			}
		}
	}
}

#nullable enable