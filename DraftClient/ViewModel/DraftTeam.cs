namespace DraftClient.ViewModel
{
    using System;

    public class DraftTeam : BindableBase
    {
        private Guid _connectedUser;
        private int _index;
        private bool _isConnected;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }

        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        public Guid ConnectedUser
        {
            get { return _connectedUser; }
            set
            {
                if (value == Guid.Empty)
                {
                    IsConnected = false;
                }
                _connectedUser = value;
            }
        }
    }
}