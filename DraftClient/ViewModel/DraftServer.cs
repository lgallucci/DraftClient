namespace DraftClient.ViewModel
{
    using System;

    public class DraftServer : BindableBase
    {
        private string _fantasyDraft;
        public string FantasyDraft
        {
            get
            {
                return _fantasyDraft;
            }
            set
            {
                SetProperty(ref this._fantasyDraft, value);
            }
        }

        private int _connectedPlayers;
        public int ConnectedPlayers
        {
            get
            {
                return _connectedPlayers;
            }
            set
            {
                SetProperty(ref this._connectedPlayers, value);
            }
        }

        private int _maxPlayers;
        public int MaxPlayers
        {
            get
            {
                return _maxPlayers;
            }
            set
            {
                SetProperty(ref this._maxPlayers, value);
            }
        }

        private string _ipAddress;
        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                SetProperty(ref this._ipAddress, value);
            }
        }

        private int _ipPort;
        public int IpPort
        {
            get
            {
                return _ipPort;
            }
            set
            {
                SetProperty(ref this._ipPort, value);
            }
        }

        private DateTime _timeout;
        public DateTime Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                SetProperty(ref this._timeout, value);
            }
        }
    }
}
