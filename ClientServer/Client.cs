namespace ClientServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using DraftEntities;

    public class Client
    {
        public Client()
        {
            IsRunning = true;
            ClientId = Guid.NewGuid();
        }

        //Send Messages on Draft Picks
        public Task ServerListener;
        private UdpClient _updClient;
        protected bool IsRunning;
        private SocketClient _client;
        public Guid ClientId { get; set; }

        #region Network Methods
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

                var multicastaddress = IPAddress.Parse(Server.MulticastAddress);
                _updClient.JoinMulticastGroup(multicastaddress);

                while (IsRunning)
                {
                    var serverBroadcastData = _updClient.Receive(ref localEp);

                    var formatter = new BinaryFormatter();
                    var networkMessage = (NetworkMessage)formatter.Deserialize(new MemoryStream(serverBroadcastData));

                    if (networkMessage.MessageType == NetworkMessageType.BroadcastMessage && networkMessage.MessageContent is DraftServer)
                    {
                        serverPingCallback((DraftServer) networkMessage.MessageContent);
                    }
                }
            });
        }

        //Listen for Draft Messages

        //Send Messages on Draft Picks

        public void ConnectToDraftServer(string ipAddress, int port)
        {
            var _tcpClient = new TcpClient();
            _tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            _client = new SocketClient(_tcpClient)
            {
                Id = ClientId
            };

            _client.ClientMessage += HandleMessage;
            _client.ClientDisconnect += HandleDisconnect;

            _client.StartClient();
        }
        #endregion

        #region Event Handlers

        private void HandleMessage(object sender, NetworkMessage networkMessage)
        {
            switch (networkMessage.MessageType)
            {
                case NetworkMessageType.LoginMessage:
                    break;
                case NetworkMessageType.LogoutMessage:
                    OnUserDisconnect(networkMessage.Id);
                    break;
                case NetworkMessageType.HandShakeMessage:
                    OnServerHandshake();
                    break;
                case NetworkMessageType.RetrieveDraftMessage:
                    OnRetrieveDraft(networkMessage.MessageContent as Draft);
                    break;
                case NetworkMessageType.RetrieveDraftSettingsMessage:
                    OnRetrieveDraftSettings(networkMessage.MessageContent as DraftSettings);
                    break;
                case NetworkMessageType.UpdateTeamMessage:
                    OnTeamUpdated(networkMessage.MessageContent as DraftTeam);
                    break;
            }
        }

        private void HandleDisconnect(object sender, Guid e)
        {
            
        }

        public virtual void SendMessage(NetworkMessageType type, object payload)
        {
            _client.SendMessage(new NetworkMessage
            {
                Id = ClientId,
                MessageType = type,
                MessageContent = payload
            });
        }

        #endregion
        
        #region Events

        public delegate void RetrieveDraftHandler(Draft draft);
        public event RetrieveDraftHandler RetrieveDraft;
        public void OnRetrieveDraft(Draft draft)
        {
            RetrieveDraftHandler handler = RetrieveDraft;
            if (handler != null)
            {
                handler(draft);
            }
        }

        public delegate Draft SendDraftHandler();
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

        public delegate void RetrieveDraftSettingsHandler(DraftSettings settings);
        public event RetrieveDraftSettingsHandler RetrieveDraftSettings;
        public void OnRetrieveDraftSettings(DraftSettings settings)
        {
            RetrieveDraftSettingsHandler handler = RetrieveDraftSettings;
            if (handler != null)
            {
                handler(settings);
            }
        }

        public delegate DraftSettings SendDraftSettingsHandler();
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

        public delegate void TeamUpdatedHandler(DraftTeam team);
        public event TeamUpdatedHandler TeamUpdated;
        public void OnTeamUpdated(DraftTeam team)
        {
            TeamUpdatedHandler handler = TeamUpdated;
            if (handler != null)
            {
                handler(team);
            }
        }

        public delegate void PickMadeHandler(Player player);
        public event PickMadeHandler PickMade;
        public void OnPickMade(Player player)
        {
            PickMadeHandler handler = PickMade;
            if (handler != null)
            {
                handler(player);
            }
        }

        public delegate void UserDisconnectHandler(Guid connectedUser);
        public event UserDisconnectHandler UserDisconnect;
        public void OnUserDisconnect(Guid connectedUser)
        {
            UserDisconnectHandler handler = UserDisconnect;
            if (handler != null)
            {
                handler(connectedUser);
            }
        }

        public delegate void ServerHandshakeHandler();
        public event ServerHandshakeHandler ServerHandshake;
        public void OnServerHandshake()
        {
            ServerHandshakeHandler handler = ServerHandshake;
            if (handler != null)
            {
                handler();
            }
        }

        #endregion

        public virtual void Close()
        {
            IsRunning = false;
            if (_updClient != null)
            {
                _updClient.Close();
                _updClient = null;
            }

            if (_client != null)
            {
                _client.ClientMessage -= HandleMessage;
                _client.ClientDisconnect -= HandleDisconnect;
                _client.Close();
            }
        }
    }
}
