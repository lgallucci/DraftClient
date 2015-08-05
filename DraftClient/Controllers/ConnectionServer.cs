namespace DraftClient.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using ClientServer;
    using DraftEntities;

    public sealed class ConnectionService
    {
        private static readonly ConnectionService instance = new ConnectionService();
        private AutoResetEvent _connectionReset;
        private Client _connection;
        private readonly bool _isRunning;
        public Task ServerListener;
        private UdpClient _updClient;

        private string address = "";
        private int port = 0;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConnectionService()
        {
        }

        private ConnectionService()
        {
            _connection = new Client();
            _isRunning = true;
            AddHandlers();
        }

        public void ListenForServers(Action<DraftServer> serverPingCallback)
        {
            ServerListener = Task.Run(() =>
            {
                _updClient = new UdpClient
                {
                    ExclusiveAddressUse = false
                };

                var localEp = new IPEndPoint(IPAddress.Any, Server.Port);

                _updClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _updClient.ExclusiveAddressUse = false;

                _updClient.Client.Bind(localEp);

                IPAddress multicastaddress = IPAddress.Parse(Server.MulticastAddress);
                _updClient.JoinMulticastGroup(multicastaddress);

                while (_isRunning)
                {
                    byte[] serverBroadcastData = _updClient.Receive(ref localEp);

                    var formatter = new BinaryFormatter();
                    var networkMessage = (NetworkMessage)formatter.Deserialize(new MemoryStream(serverBroadcastData));

                    if (networkMessage.MessageType == NetworkMessageType.ServerBroadcast && networkMessage.MessageContent is DraftServer)
                    {
                        serverPingCallback((DraftServer)networkMessage.MessageContent);
                    }
                }
            });
        }

        public static ConnectionService Instance
        {
            get { return instance; }
        }

        public bool IsConnected { get { return _connection.IsConnected; } }

        public void StartServer(string leagueName, int numberOfTeams)
        {
            RemoveHandlers();
            _connection.Close();
            _connection = new Server(leagueName, numberOfTeams);
            ((Server) _connection).StartServer();
            AddHandlers();
        }

        public void ResetConnection()
        {
            RemoveHandlers();
            _connection.Close();
            _connection = null;
            _connection = new Client();
            AddHandlers();
        }

        private void AddHandlers()
        {
            _connection.ServerHandshake += ServerHandshake;
            _connection.RetrieveDraftSettings += OnRetrieveDraftSettings;
            _connection.TeamUpdated += OnTeamUpdated;
            _connection.PickMade += OnPickMade;
            _connection.TeamUpdated += OnTeamUpdated;
            _connection.SendDraftSettings += OnSendDraftSettings;
            _connection.UserDisconnect += OnUserDisconnect;
            _connection.DraftStop += OnDraftStop;
            _connection.DraftStateChanged += OnDraftStateChanged;
            _connection.Disconnect += OnDisconnect;
        }

        private void RemoveHandlers()
        {
            _connection.ServerHandshake -= ServerHandshake;
            _connection.RetrieveDraftSettings -= OnRetrieveDraftSettings;
            _connection.TeamUpdated -= OnTeamUpdated;
            _connection.PickMade -= OnPickMade;
            _connection.TeamUpdated -= OnTeamUpdated;
            _connection.SendDraftSettings -= OnSendDraftSettings;
            _connection.UserDisconnect -= OnUserDisconnect;
            _connection.DraftStop -= OnDraftStop;
            _connection.DraftStateChanged -= OnDraftStateChanged;
            _connection.Disconnect -= OnDisconnect;
        }

        public void ConnectToDraft()
        {
            if (string.IsNullOrWhiteSpace(address) || port <= 0)
            {
                throw new ArgumentException("IP and Port not set");
            }

            ConnectToDraft(address, port);
        }

        public void ConnectToDraft(string ipAddress, int ipPort)
        {
            address = ipAddress;
            port = ipPort;

            _connection.ConnectToDraftServer(address, port);

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
            var server = _connection as Server;
            if (server != null)
            {
                server.BroadcastMessage(sendDraftMessage, payload);
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

        #region Events

        public delegate void PickMadeHandler(DraftPick pick);

        public delegate void RetrieveDraftHandler(Draft draft);

        public delegate void RetrieveDraftSettingsHandler(DraftSettings settings);

        public delegate Draft SendDraftHandler();

        public delegate DraftSettings SendDraftSettingsHandler();

        public delegate void TeamUpdatedHandler(DraftTeam team);

        public delegate void UserDisconnectHandler(Guid connectedUser);

        public delegate void DraftStopHandler();

        public delegate void DraftStateChangedHandler(DraftState state);

        public delegate void DisconnectHandler();

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

        public event DraftStopHandler DraftStop;

        public void OnDraftStop()
        {
            DraftStopHandler handler = DraftStop;
            if (handler != null)
            {
                handler();
            }
        }

        public event DraftStateChangedHandler DraftStateChanged;

        public void OnDraftStateChanged(DraftState state)
        {
            DraftStateChangedHandler handler = DraftStateChanged;
            if (handler != null)
            {
                handler(state);
            }
        }
        
        public event DisconnectHandler Disconnect;

        public void OnDisconnect()
        {
            DisconnectHandler handler = Disconnect;
            if (handler != null)
            {
                handler();
            }
        }

        #endregion
    }
}