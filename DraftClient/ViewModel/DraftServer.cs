namespace DraftClient.ViewModel
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        private IPEndPoint _tcpAddress;
        public IPEndPoint tcpAddress
        {
            get
            {
                return _tcpAddress;
            }
            set
            {
                SetProperty(ref this._tcpAddress, value);
            }
        }
    }
}
