namespace DraftClient.Controllers
{
    using System;
    using System.Threading;
    using ClientServer;
    using DraftEntities;

    public sealed class ConnectionServer
    {
        private static readonly ConnectionServer instance = new ConnectionServer();
        private AutoResetEvent _connectionReset;
        private Client _connection;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConnectionServer()
        {
        }

        private ConnectionServer()
        {
            _connection = new Client();
            AddHandlers();
        }

        public static ConnectionServer Instance
        {
            get { return instance; }
        }

        public bool IsConnected { get { return _connection.IsConnected; } }

        public void StartServer(string leagueName, int numberOfTeams)
        {
            RemoveHandlers();
            _connection = new Server(leagueName, numberOfTeams);
            ((Server) _connection).StartServer();
            AddHandlers();
        }

        public void ResetConnection()
        {
            RemoveHandlers();
            _connection.Close();
            _connection = new Client();
            AddHandlers();
        }

        private void AddHandlers()
        {
            _connection.ServerHandshake += ServerHandshake;
            _connection.RetrieveDraftSettings += OnRetrieveDraftSettings;
            _connection.TeamUpdated += OnTeamUpdated;
            _connection.PickMade += OnPickMade;
            _connection.SendDraft += OnSendDraft;
            _connection.TeamUpdated += OnTeamUpdated;
            _connection.SendDraftSettings += OnSendDraftSettings;
            _connection.UserDisconnect += OnUserDisconnect;
            _connection.RetrieveDraft += OnRetrieveDraft;
        }

        private void RemoveHandlers()
        {
            _connection.ServerHandshake -= ServerHandshake;
            _connection.RetrieveDraftSettings -= OnRetrieveDraftSettings;
            _connection.TeamUpdated -= OnTeamUpdated;
            _connection.PickMade -= OnPickMade;
            _connection.SendDraft -= OnSendDraft;
            _connection.TeamUpdated -= OnTeamUpdated;
            _connection.SendDraftSettings -= OnSendDraftSettings;
            _connection.UserDisconnect -= OnUserDisconnect;
            _connection.RetrieveDraft -= OnRetrieveDraft;
        }

        public void ConnectToDraft(string ipAddress, int ipPort)
        {
            _connection.ConnectToDraftServer(ipAddress, ipPort);

            _connectionReset = new AutoResetEvent(false);
            _connection.SendMessage(NetworkMessageType.LoginMessage, _connection.ClientId.ToString());

            if (!_connectionReset.WaitOne(5000))
            {
                throw new TimeoutException("Couldn't connect to draft server");
            }
        }

        private void ServerHandshake()
        {
            _connectionReset.Set();
        }

        public void SendMessage(NetworkMessageType sendDraftMessage, object payload)
        {
            if (_connection is Server)
            {
                ((Server)_connection).BroadcastMessage(sendDraftMessage, payload);
            }
            else
            {
                _connection.SendMessage(sendDraftMessage, payload);
            }
        }
  
        public Guid GetClientId()
        {
            return _connection.ClientId;
        }

        public void ListenForServers(Action<DraftEntities.DraftServer> action)
        {
            _connection.ListenForServers(action);
        }


        #region Events

        public delegate void PickMadeHandler(DraftPick pick);

        public delegate void RetrieveDraftHandler(Draft draft);

        public delegate void RetrieveDraftSettingsHandler(DraftSettings settings);

        public delegate Draft SendDraftHandler();

        public delegate DraftSettings SendDraftSettingsHandler();

        public delegate void TeamUpdatedHandler(DraftTeam team);

        public delegate void UserDisconnectHandler(Guid connectedUser);

        public event RetrieveDraftHandler RetrieveDraft;

        public void OnRetrieveDraft(Draft draft)
        {
            RetrieveDraftHandler handler = RetrieveDraft;
            if (handler != null)
            {
                handler(draft);
            }
        }

        public event SendDraftHandler SendDraft;

        public Draft OnSendDraft()
        {
            SendDraftHandler handler = SendDraft;
            if (handler != null)
            {
                return handler();
            }
            return null;
        }

        public event RetrieveDraftSettingsHandler RetrieveDraftSettings;

        public void OnRetrieveDraftSettings(DraftSettings settings)
        {
            RetrieveDraftSettingsHandler handler = RetrieveDraftSettings;
            if (handler != null)
            {
                handler(settings);
            }
        }

        public event SendDraftSettingsHandler SendDraftSettings;

        public DraftSettings OnSendDraftSettings()
        {
            SendDraftSettingsHandler handler = SendDraftSettings;
            if (handler != null)
            {
                return handler();
            }
            return null;
        }

        public event TeamUpdatedHandler TeamUpdated;

        public void OnTeamUpdated(DraftTeam team)
        {
            TeamUpdatedHandler handler = TeamUpdated;
            if (handler != null)
            {
                handler(team);
            }
        }

        public event PickMadeHandler PickMade;

        public void OnPickMade(DraftPick pick)
        {
            PickMadeHandler handler = PickMade;
            if (handler != null)
            {
                handler(pick);
            }
        }

        public event UserDisconnectHandler UserDisconnect;

        public void OnUserDisconnect(Guid connectedUser)
        {
            UserDisconnectHandler handler = UserDisconnect;
            if (handler != null)
            {
                handler(connectedUser);
            }
        }

        #endregion
    }
}