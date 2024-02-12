using Tmds.DBus;

namespace UDisks2.DBus
{
	[Dictionary]
    class SwapspaceProperties
    {
        private bool _Active = default(bool);
        public bool Active
        {
            get
            {
                return _Active;
            }

            set
            {
                _Active = (value);
            }
        }
    }
}