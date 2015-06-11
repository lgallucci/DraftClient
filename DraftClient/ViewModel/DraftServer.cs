namespace DraftClient.ViewModel
{
    using System;

    public class DraftServer : BindableBase
    {
        private int _connectedPlayers;
        private string _fantasyDraft;
        private string _ipAddress;
        private int _ipPort;
        private int _maxPlayers;
        private DateTime _timeout;

        public string FantasyDraft
        {
            get { return _fantasyDraft; }
            set { SetProperty(ref _fantasyDraft, value); }
        }

        public int ConnectedPlayers
        {
            get { return _connectedPlayers; }
            set { SetProperty(ref _connectedPlayers, value); }
        }

        public int MaxPlayers
        {
            get { return _maxPlayers; }
            set { SetProperty(ref _maxPlayers, value); }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { SetProperty(ref _ipAddress, value); }
        }

        public int IpPort
        {
            get { return _ipPort; }
            set { SetProperty(ref _ipPort, value); }
        }

        public DateTime Timeout
        {
            get { return _timeout; }
            set { SetProperty(ref _timeout, value); }
        }
    }
}