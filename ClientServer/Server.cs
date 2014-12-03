using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ClientServer
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class Server
    {
        private readonly string leagueName;
        private readonly int numberOfTeams;
        private Dictionary<int, ClientConnections> connections;
        private static TcpListener _listener;

        public bool IsRunning { get; set; }

        public Server(string leagueName, int numberOfTeams)
        {
            this.leagueName = leagueName;
            this.numberOfTeams = numberOfTeams;
            connections = new Dictionary<int, ClientConnections>();
        }

        //SetupServer
        public Server()
        {
            //Start Broadcast Loop
        }
        
        //ListenForMessages

        //ControlDraft
        
        public void StartServer()
        {
            var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes(string.Format("Fantasy Draft Server: {0}, {1}/{2}", leagueName, connections.Count, numberOfTeams));

            IPAddress localIPAddress = IPAddress.Parse("10.3.145.112");
            var ipLocal = new IPEndPoint(localIPAddress, 8888);
            _listener = new TcpListener(ipLocal);
            _listener.Start();
            WaitForClientConnect();

            IsRunning = true;

            while (IsRunning)
            {
                Client.EnableBroadcast = true;
                Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
                Thread.Sleep(5000);
            }
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
            catch (Exception se)
            {
                throw;
            }

            WaitForClientConnect();
        }

    }
}
