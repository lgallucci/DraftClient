namespace ClientServer
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Linq;
    using System.Xml.Serialization;
    using System.IO;
    using System.Threading.Tasks;
    using DraftEntities;

    public class Server
    {
        private readonly string leagueName;
        private readonly int numberOfTeams;
        private Dictionary<int, ClientConnections> connections;
        private static TcpListener _listener;
        private int _port;

        public bool IsRunning { get; set; }

        public Server(string leagueName, int numberOfTeams)
        {
            this.leagueName = leagueName;
            this.numberOfTeams = numberOfTeams;
            connections = new Dictionary<int, ClientConnections>();
            _port = 11000;
        }

        //ListenForMessages

        //ControlDraft

        public void StartServer()
        {
            var udpclient = new UdpClient();
            IsRunning = true;
            byte[] RequestData;
            var xmlSerializer = new XmlSerializer(typeof(DraftServer));
            var ipAddress = GetFirstIPAddress();

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
                        FantasyDraft = leagueName,
                        ConnectedPlayers = connections.Count((x) => x.Value.TcpClient.Connected),
                        MaxPlayers = numberOfTeams,
                        ipAddress = ipAddress,
                        ipPort = _port
                    };

                    using (var memoryStream = new MemoryStream())
                    {
                        xmlSerializer.Serialize(memoryStream, draftServer);
                        RequestData = memoryStream.ToArray();
                    }

                    udpclient.EnableBroadcast = true;
                    udpclient.Send(RequestData, RequestData.Length, remoteep);
                    Thread.Sleep(5000);
                }
            });
        }

        private void WaitForClientConnect()
        {
            var obj = new object();
            _listener.BeginAcceptTcpClient(OnClientConnect, obj);
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = _listener.EndAcceptTcpClient(asyn);
                var clientReq = new HandleClientRequest(clientSocket);
                clientReq.StartClient();
            }
            catch
            {
                throw;
            }

            WaitForClientConnect();
        }

        /// <summary> 
        /// This utility function displays all the IP (v4, not v6) addresses of the local computer. 
        /// </summary> 
        public string GetFirstIPAddress()
        {
            StringBuilder sb = new StringBuilder();

            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network 
                IPInterfaceProperties properties = network.GetIPProperties();

                // Each network interface may have multiple IP addresses 
                foreach (IPAddressInformation address in properties.UnicastAddresses)
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
