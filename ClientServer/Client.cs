namespace ClientServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;
    using DraftEntities;

    public class Client
    {
        private ConnectedClient _client;
        private readonly Timer _timKeepAlive;
        private readonly Timer _timAcknowledgeReturn;

        internal sealed class TimeoutMessage
        {
            public DateTime Timeout { get; set; }
            public ConnectedClient ConnectedClient { get; set; }
            public NetworkMessage Message { get; set; }
        }
        internal ConcurrentDictionary<Guid, TimeoutMessage> SentMessages;

        public Client()
        {
            ClientId = Guid.NewGuid();
            _timKeepAlive = new Timer();
            SentMessages = new ConcurrentDictionary<Guid, TimeoutMessage>();

            _timAcknowledgeReturn = new Timer
            {
                Interval = 250,
                Enabled = true
            };
            _timAcknowledgeReturn.Elapsed += (sender, args) =>
            {
                foreach (var message in SentMessages.Where(sm => sm.Value.Timeout < DateTime.Now).ToList())
                {
                    //Console.WriteLine("MessageCount:{0}", SentMessages.Count);
                    TimeoutMessage missedMessage;
                    SentMessages.TryRemove(message.Value.Message.MessageId, out missedMessage);
                    SendMessage(missedMessage.ConnectedClient, missedMessage.Message);
                }
            };
        }

        public Guid ClientId { get; set; }

        public bool IsConnected
        {
            get { return _client.Client.Connected; }
        }

        #region Network Methods

        public void ConnectToDraftServer(string ipAddress, int port)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            _client = new ConnectedClient
            {
                Client = new SocketClient(tcpClient)
                {
                    Id = ClientId
                }
            };
            _timAcknowledgeReturn.Start();

            _client.Client.ClientMessage += HandleMessage;
            _client.Client.ClientDisconnect += HandleDisconnect;

            _client.Client.StartClient();
        }

        #endregion

        #region Event Handlers

        private void HandleMessage(object sender, NetworkMessage networkMessage)
        {
            //Console.WriteLine("Recieve Msg Type: {0}, Id: {1}", networkMessage.MessageType.ToString(), networkMessage.MessageId);
            try
            {
                switch (networkMessage.MessageType)
                {
                    case NetworkMessageType.Ackgnowledge:
                        TimeoutMessage ackedMessage;
                        SentMessages.TryRemove((Guid)networkMessage.MessageContent, out ackedMessage);
                        return;
                    case NetworkMessageType.LogoutMessage:
                        OnUserDisconnect(networkMessage.SenderId);
                        break;
                    case NetworkMessageType.HandShakeMessage:

                        _timKeepAlive.Elapsed += (o, args) => SendMessage(NetworkMessageType.KeepAliveMessage, null);
                        _timKeepAlive.Interval = 5000;
                        _timKeepAlive.Enabled = true;
                        _timKeepAlive.Start();

                        OnServerHandshake();
                        break;
                    case NetworkMessageType.RetrieveDraftSettingsMessage:
                        OnRetrieveDraftSettings(networkMessage.MessageContent as DraftSettings);
                        break;
                    case NetworkMessageType.UpdateTeamMessage:
                        OnTeamUpdated(networkMessage.MessageContent as DraftTeam);
                        break;
                    case NetworkMessageType.PickMessage:
                        OnPickMade(networkMessage.MessageContent as DraftPick);
                        break;
                    case NetworkMessageType.DraftStopMessage:
                        OnDraftStop();
                        break;
                    case NetworkMessageType.UpdateDraftState:
                        OnDraftStateChanged(networkMessage.MessageContent as DraftState);
                        break;
                }

                //Console.WriteLine("Sent Ack Type: {0}, Id: {1}", networkMessage.MessageType.ToString(), networkMessage.MessageId);
                SendMessage(NetworkMessageType.Ackgnowledge, networkMessage.MessageId);
            }
            catch (Exception ex)
            {
                //TODO: Handle exceptions on network handling
            }
        }

        private void HandleDisconnect(object sender, Guid e)
        {
            //OnDisconnect();

            SentMessages.Clear();
            _timKeepAlive.Stop();
            _timAcknowledgeReturn.Stop();
        }

        public virtual void SendMessage(NetworkMessageType type, object payload)
        {
            if (_client != null)
            {
                var networkMessage = new NetworkMessage
                {
                    SenderId = ClientId,
                    MessageType = type,
                    MessageContent = payload
                };

                SendMessage(_client, networkMessage);
            }
        }

        internal virtual void SendMessage(ConnectedClient connection, NetworkMessage message)
        {
            if (message.MessageType != NetworkMessageType.Ackgnowledge && message.MessageType != NetworkMessageType.KeepAliveMessage)
            {
                SentMessages.TryAdd(message.MessageId, new TimeoutMessage
                {
                    ConnectedClient = connection,
                    Message = message,
                    Timeout = DateTime.Now.AddSeconds(10)
                });
            }

            connection.Client.SendMessage(message);
        }

        #endregion

        #region Events

        public delegate void PickMadeHandler(DraftPick pick);

        public delegate void RetrieveDraftSettingsHandler(DraftSettings settings);
        
        public delegate DraftSettings SendDraftSettingsHandler();

        public delegate void ServerHandshakeHandler();

        public delegate void TeamUpdatedHandler(DraftTeam team);

        public delegate void UserDisconnectHandler(Guid connectedUser);

        public delegate void DraftStopHandler();

        public delegate void DraftStateChangedHandler(DraftState state);

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

        public event ServerHandshakeHandler ServerHandshake;

        public void OnServerHandshake()
        {
            ServerHandshakeHandler handler = ServerHandshake;
            if (handler != null)
            {
                handler();
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

        #endregion

        public virtual void Close()
        {
            SentMessages.Clear();
            _timKeepAlive.Stop();
            _timAcknowledgeReturn.Stop();

            if (_client != null)
            {
                _client.Client.ClientMessage -= HandleMessage;
                _client.Client.ClientDisconnect -= HandleDisconnect;
                _client.Client.Close();
            }
        }
    }
}