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

    public class Server : Client
    {
        private readonly string _leagueName;
        private readonly int _numberOfTeams;
        public readonly Collection<SocketClient> Connections;
        private static TcpListener _listener;
        private readonly int _port;
        private readonly System.Timers.Timer _timKeepAlive;

        public static int Port = 11000;
        public static string MulticastAddress = "239.0.0.222";

        public Server(string leagueName, int numberOfTeams)
        {
            _leagueName = leagueName;
            _numberOfTeams = numberOfTeams;
            Connections = new Collection<SocketClient>();
            _timKeepAlive = new System.Timers.Timer();

            _port = Server.Port;
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
                IPAddress multicastaddress = IPAddress.Parse(Server.MulticastAddress);
                udpclient.JoinMulticastGroup(multicastaddress);
                var remoteep = new IPEndPoint(multicastaddress, _port);

                while (IsRunning)
                {
                    var draftServer = new DraftServer()
                    {
                        FantasyDraft = _leagueName,
                        ConnectedPlayers = Connections.Count((x) => x.LoggedIn),
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

        public void StopServer()
        {
            IsRunning = false;
            foreach (var connection in Connections)
            {
                connection.SendMessage(new NetworkMessage { MessageType = NetworkMessageType.DraftStopMessage });
                connection.OnClientLogin -= ClientLoginHandler;
                connection.OnClientLogout -= ClientLogoutHandler;
                connection.OnClientPick -= ClientPickHandler;
            }
        }

        private void WaitForClientConnect()
        {
            var obj = new object();
            _listener.BeginAcceptTcpClient(OnClientConnect, obj);
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            TcpClient clientSocket = default(TcpClient);
            clientSocket = _listener.EndAcceptTcpClient(asyn);
            var clientReq = new SocketClient(clientSocket);

            clientReq.OnClientLogin += ClientLoginHandler;
            clientReq.OnClientLogout += ClientLogoutHandler;
            clientReq.OnClientPick += ClientPickHandler;

            Connections.Add(clientReq);

            clientReq.StartClient();

            WaitForClientConnect();
        }

        private void KeepSocketsAlive(object sender, ElapsedEventArgs e)
        {
            foreach (var connection in Connections)
            {
                connection.SendMessage(new NetworkMessage
                {
                    MessageType = NetworkMessageType.KeepAliveMessage
                });
            }
        }

        #region Event Handlers

        private void ClientLoginHandler(object sender, ClientLoginEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ClientLogoutHandler(object sender, ClientLogoutEventArgs e)
        {
            Connections.Remove((SocketClient) sender);
        }

        private void ClientPickHandler(object sender, ClientPickEventArgs e)
        {
            //throw new NotImplementedException();
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
