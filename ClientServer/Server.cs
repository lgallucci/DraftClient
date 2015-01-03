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
    using System.Timers;
    using DraftEntities;

    public class ConnectedClient
    {
        public SocketClient Client;
        public bool LoggedIn;
        public string ClientName;
    }

    public class Server : Client
    {
        private readonly string _leagueName;
        private readonly int _numberOfTeams;
        private static TcpListener _listener;
        private readonly int _port;
        private readonly System.Timers.Timer _timKeepAlive;

        public static int Port = 11000;
        public static string MulticastAddress = "239.0.0.222";
        public readonly Collection<ConnectedClient> Connections;

        public Server(string leagueName, int numberOfTeams)
        {
            _leagueName = leagueName;
            _numberOfTeams = numberOfTeams;
            Connections = new Collection<ConnectedClient>();
            _timKeepAlive = new System.Timers.Timer();

            _port = Port;
        }

        public void StartServer()
        {
            var udpclient = new UdpClient();
            IsRunning = true;
            var formatter = new BinaryFormatter();
            var ipAddress = GetFirstIpAddress();

            Task.Run(() =>
            {
                _listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ipAddress), _port));
                _listener.Start();
                WaitForClientConnect();
            });
            
            _timKeepAlive.Elapsed += KeepSocketsAlive;
            _timKeepAlive.Interval = 2000;
            _timKeepAlive.Enabled = true;

            Task.Run(() =>
            {
                IPAddress multicastaddress = IPAddress.Parse(MulticastAddress);
                udpclient.JoinMulticastGroup(multicastaddress);
                var remoteep = new IPEndPoint(multicastaddress, _port);

                while (IsRunning)
                {
                    var draftServer = new DraftServer()
                    {
                        FantasyDraft = _leagueName,
                        ConnectedPlayers = Connections.Count((x) => x.LoggedIn) + 1,
                        MaxPlayers = _numberOfTeams,
                        IpAddress = ipAddress,
                        IpPort = _port
                    };

                    byte[] requestData;
                    using (var memoryStream = new MemoryStream())
                    {
                        formatter.Serialize(memoryStream, new NetworkMessage { MessageType = NetworkMessageType.BroadcastMessage, MessageContent = draftServer });
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
            IsRunning = false;
            foreach (var connection in Connections)
            {
                connection.Client.SendMessage(new NetworkMessage { MessageType = NetworkMessageType.DraftStopMessage });
                connection.Client.ClientMessage -= HandleMessage;
                connection.Client.ClientDisconnect -= HandleDisconnect;
                connection.Client.Close();
            }
        }

        private void WaitForClientConnect()
        {
            var obj = new object();
            _listener.BeginAcceptTcpClient(OnClientConnect, obj);
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            TcpClient tcpClient = _listener.EndAcceptTcpClient(asyn);
            var socketClient = new SocketClient(tcpClient);

            socketClient.ClientMessage += HandleMessage;
            socketClient.ClientDisconnect += HandleDisconnect;

            Connections.Add(new ConnectedClient
            {
                Client = socketClient
            });

            socketClient.StartClient();

            WaitForClientConnect();
        }

        private void KeepSocketsAlive(object sender, ElapsedEventArgs e)
        {
            foreach (var connection in Connections)
            {
                connection.Client.SendMessage(new NetworkMessage
                {
                    MessageType = NetworkMessageType.KeepAliveMessage
                });
            }
        }

        #region Event Handlers

        private void HandleMessage(object sender, NetworkMessage networkMessage)
        {
            if (networkMessage.MessageType == NetworkMessageType.LoginMessage)
            {
                if (networkMessage.MessageContent is String)
                {
                    ConnectedClient connection = Connections.FirstOrDefault(c => c.Client == (SocketClient)sender);
                    if (connection != null)
                    {
                        connection.LoggedIn = true;
                        connection.ClientName = networkMessage.MessageContent.ToString();
                    }
                }
            }
            else if (networkMessage.MessageType == NetworkMessageType.LogoutMessage)
            {
                Logout((SocketClient)sender);
                }
            else if (networkMessage.MessageType == NetworkMessageType.PickMessage)
            {
                if (networkMessage.MessageContent is Player)
                {
                    //TODO: Handle Pick
                }
            }
        }

        private void HandleDisconnect(object sender, EventArgs e)
        {
            Logout((SocketClient)sender);
        }

        private void Logout(SocketClient sender)
        {
            ConnectedClient connection = Connections.FirstOrDefault(c => c.Client == sender);
            if (connection != null)
            {
                Connections.Remove(connection);
            }
        }

        #endregion

        #region Helper Methods
        /// <summary> 
        /// This utility function displays all the IP (v4, not v6) addresses of the local computer. 
        /// </summary> 
        public string GetFirstIpAddress()
        {
            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                // Read the IP configuration for each network 
                IPInterfaceProperties properties = network.GetIPProperties();

                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;

                // Each network interface may have multiple IP addresses 
                foreach (var address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now 
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    // Ignore loopback addresses (e.g., 127.0.0.1) 
                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    return address.Address.ToString();
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
