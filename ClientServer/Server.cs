namespace ClientServer
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using DraftEntities;

    public class Server : Client
    {
        public static int Port = 11000;
        public static string MulticastAddress = "239.0.0.222";
        public readonly Collection<ConnectedClient> Connections;

        private readonly object _connectionLock = new object();
        private readonly string _leagueName;
        private readonly int _numberOfTeams;
        private readonly int _port;
        private TcpListener _listener;
        private bool _isRunning;

        public Server(string leagueName, int numberOfTeams)
        {
            _leagueName = leagueName;
            _numberOfTeams = numberOfTeams;
            Connections = new Collection<ConnectedClient>();

            _port = Port;
        }

        public void StartServer()
        {
            var udpclient = new UdpClient();
            _isRunning = true;
            var formatter = new BinaryFormatter();
            string ipAddress = GetFirstIpAddress();

            Task.Run(() =>
            {
                _listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ipAddress), _port));
                _listener.Start();
                WaitForClientConnect();
            });

            Task.Run(() =>
            {
                IPAddress multicastaddress = IPAddress.Parse(MulticastAddress);
                udpclient.JoinMulticastGroup(multicastaddress);
                var remoteep = new IPEndPoint(multicastaddress, _port);

                while (_isRunning)
                {
                    int connectionCount;
                    lock (_connectionLock)
                    {
                        connectionCount = Connections.Count() + 1;
                    }

                    var draftServer = new DraftServer
                    {
                        FantasyDraft = _leagueName,
                        ConnectedPlayers = connectionCount,
                        MaxPlayers = _numberOfTeams,
                        IpAddress = ipAddress,
                        IpPort = _port
                    };

                    byte[] requestData;
                    using (var memoryStream = new MemoryStream())
                    {
                        formatter.Serialize(memoryStream, new NetworkMessage
                        {
                            MessageType = NetworkMessageType.ServerBroadcast,
                            MessageContent = draftServer
                        });
                        requestData = memoryStream.ToArray();
                    }

                    udpclient.EnableBroadcast = true;
                    udpclient.Send(requestData, requestData.Length, remoteep);
                    Thread.Sleep(5000);
                }
            });
        }

        public override void Close()
        {
            _isRunning = false;
            _listener.Stop();
            _listener = null;
            lock (_connectionLock)
            {
                foreach (ConnectedClient connection in Connections)
                {
                    connection.Client.SendMessage(new NetworkMessage
                    {
                        MessageType = NetworkMessageType.DraftStopMessage
                    });
                    connection.Client.ClientMessage -= HandleMessage;
                    connection.Client.ClientDisconnect -= HandleDisconnect;
                    connection.Client.Close();
                }
            }
            base.Close();
        }

        public override void SendMessage(NetworkMessageType type, object payload)
        {
            BroadcastMessage(new NetworkMessage
            {
                SenderId = ClientId,
                MessageType = type,
                MessageContent = payload
            });
        }

        private void WaitForClientConnect()
        {
            var obj = new object();
            _listener.BeginAcceptTcpClient(OnClientConnect, obj);
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            if (!_isRunning) return;

            TcpClient tcpClient = _listener.EndAcceptTcpClient(asyn);
            var socketClient = new SocketClient(tcpClient);

            socketClient.ClientMessage += HandleMessage;
            socketClient.ClientDisconnect += HandleDisconnect;

            lock (_connectionLock)
            {
                Connections.Add(new ConnectedClient
                {
                    Client = socketClient
                });
            }

            socketClient.StartClient();

            WaitForClientConnect();
        }

        #region Event Handlers

        private void HandleMessage(object sender, NetworkMessage networkMessage)
        {
            //Console.WriteLine("Recieve Msg Type: {0}, Id: {1}", networkMessage.MessageType.ToString(), networkMessage.MessageId);
            try
            {
                ConnectedClient connection;
                lock (_connectionLock)
                {
                    connection = Connections.FirstOrDefault(c => c.Client == (SocketClient)sender);
                }
                switch (networkMessage.MessageType)
                {
                    case NetworkMessageType.Ackgnowledge:
                        TimeoutMessage ackedMessage;
                        SentMessages.TryRemove((Guid)networkMessage.MessageContent, out ackedMessage);
                        return;
                    case NetworkMessageType.LoginMessage:
                        if (connection != null)
                        {
                            connection.Id = new Guid(networkMessage.MessageContent.ToString());
                        }
                        SendMessage(connection, new NetworkMessage
                        {
                            SenderId = ClientId,
                            MessageType = NetworkMessageType.HandShakeMessage
                        });
                        break;
                    case NetworkMessageType.LogoutMessage:
                        Logout(connection);
                        break;
                    case NetworkMessageType.SendDraftSettingsMessage:
                        DraftSettings draftSettings = OnSendDraftSettings();
                        SendMessage(connection, new NetworkMessage
                        {
                            SenderId = ClientId,
                            MessageContent = draftSettings,
                            MessageType = NetworkMessageType.RetrieveDraftSettingsMessage
                        });
                        break;
                    case NetworkMessageType.UpdateTeamMessage:
                        var draftTeam = networkMessage.MessageContent as DraftTeam;
                        if (draftTeam != null)
                        {
                            OnTeamUpdated(draftTeam);
                        }
                        BroadcastMessage(networkMessage);
                        break;
                    case NetworkMessageType.PickMessage:
                        var pick = networkMessage.MessageContent as DraftPick;
                        if (pick != null)
                        {
                            OnPickMade(pick);
                        }
                        BroadcastMessage(networkMessage);
                        break;
                }

                //Console.WriteLine("Sent Ack Type: {0}, Id: {1}", networkMessage.MessageType.ToString(), networkMessage.MessageId);
                SendMessage(connection, new NetworkMessage
                {
                    MessageType = NetworkMessageType.Ackgnowledge,
                    MessageContent = networkMessage.MessageId
                });
            }
            catch (Exception)
            {
                HandleDisconnect(sender, ClientId);
            }
        }

        public void BroadcastMessage(NetworkMessageType type, object payload)
        {
            BroadcastMessage(new NetworkMessage
            {
                SenderId = ClientId,
                MessageContent = payload,
                MessageType = type
            });
        }

        private void BroadcastMessage(NetworkMessage networkMessage)
        {
            lock (_connectionLock)
            {
                foreach (ConnectedClient connection in Connections.Where(c => c.Id != networkMessage.SenderId))
                {
                    SendMessage(connection, networkMessage);
                }
            }
        }

        internal override void SendMessage(ConnectedClient connection, NetworkMessage networkMessage)
        {
            if (connection != null)
            {
                if (networkMessage.MessageType != NetworkMessageType.Ackgnowledge)
                {
                    SentMessages.TryAdd(networkMessage.MessageId, new TimeoutMessage
                    {
                        ConnectedClient = connection,
                        Message = networkMessage,
                        Timeout = DateTime.Now.AddSeconds(10)
                    });
                }

                connection.Client.SendMessage(networkMessage);
            }
        }

        private void HandleDisconnect(object sender, Guid id)
        {
            lock (_connectionLock)
            {
                ConnectedClient connection = Connections.FirstOrDefault(c => c.Client == (SocketClient)sender);
                Logout(connection);
            }
        }

        private void Logout(ConnectedClient connection)
        {
            if (connection != null)
            {
                OnUserDisconnect(connection.Id);

                BroadcastMessage(new NetworkMessage
                {
                    SenderId = connection.Id,
                    MessageType = NetworkMessageType.LogoutMessage
                });
                lock (_connectionLock)
                {
                    Connections.Remove(connection);
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     This utility function displays all the IP (v4, not v6) addresses of the local computer.
        /// </summary>
        public string GetFirstIpAddress()
        {
            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network 
                IPInterfaceProperties properties = network.GetIPProperties();

                if (network.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                // Each network interface may have multiple IP addresses 
                foreach (UnicastIPAddressInformation address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now 
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    // Ignore loopback addresses (e.g., 127.0.0.1) 
                    if (IPAddress.IsLoopback(address.Address))
                    {
                        continue;
                    }

                    return address.Address.ToString();
                }
            }
            return string.Empty;
        }

        #endregion
    }
}