namespace ClientServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using DraftEntities;

    public class Server : Client
    {
        private readonly string _leagueName;
        private readonly int _numberOfTeams;
        private readonly Dictionary<int, ClientConnections> _connections;
        private static TcpListener _listener;
        private readonly int _port;

        public Server(string leagueName, int numberOfTeams)
        {
            _leagueName = leagueName;
            _numberOfTeams = numberOfTeams;
            _connections = new Dictionary<int, ClientConnections>();
            _port = 11000;
        }

        public void StartServer()
        {
            var udpclient = new UdpClient();
            IsRunning = true;
            byte[] requestData;
            var xmlSerializer = new XmlSerializer(typeof(DraftServer));
            var ipAddress = GetFirstIpAddress();

            Task.Run(() =>
            {
                _listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ipAddress), _port));
                _listener.Start();
                WaitForClientConnect();
            });
             
            Task.Run(() =>
            {
                IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
                udpclient.JoinMulticastGroup(multicastaddress);
                IPEndPoint remoteep = new IPEndPoint(multicastaddress, _port);

                while (IsRunning)
                {
                    var draftServer = new DraftServer()
                    {
                        FantasyDraft = _leagueName,
                        ConnectedPlayers = _connections.Count((x) => x.Value.TcpClient.Connected),
                        MaxPlayers = _numberOfTeams,
                        IpAddress = ipAddress,
                        IpPort = _port
                    };

                    using (var memoryStream = new MemoryStream())
                    {
                        xmlSerializer.Serialize(memoryStream, draftServer);
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
        }

        //ListenForMessages

        private void WaitForClientConnect()
        {
            var obj = new object();
            _listener.BeginAcceptTcpClient(OnClientConnect, obj);
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            TcpClient clientSocket = default(TcpClient);
            clientSocket = _listener.EndAcceptTcpClient(asyn);
            var clientReq = new HandleClientRequest(clientSocket);
            clientReq.StartClient();

            WaitForClientConnect();
        }

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
    }
}
