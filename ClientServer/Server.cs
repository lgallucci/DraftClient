using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class Server
    {
        private readonly string leagueName;
        private readonly int numberOfTeams;
        private Dictionary<int, TcpClient> connections;

        private static TcpListener _listener;

        public Server(string leagueName, int numberOfTeams)
        {
            this.leagueName = leagueName;
            this.numberOfTeams = numberOfTeams;
            connections = new Dictionary<int, TcpClient>();
        }

        //SetupServer

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

            while (true)
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

    public class HandleClientRequest
    {
        TcpClient _clientSocket;
        NetworkStream _networkStream = null;
        public HandleClientRequest(TcpClient clientConnected)
        {
            this._clientSocket = clientConnected;
        }
        public void StartClient()
        {
            _networkStream = _clientSocket.GetStream();
            WaitForRequest();
        }

        public void WaitForRequest()
        {
            byte[] buffer = new byte[_clientSocket.ReceiveBufferSize];

            _networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream = _clientSocket.GetStream();
            try
            {
                int read = networkStream.EndRead(result);
                if (read == 0)
                {
                    _networkStream.Close();
                    _clientSocket.Close();
                    return;
                }

                byte[] buffer = result.AsyncState as byte[];
                string data = Encoding.Default.GetString(buffer, 0, read);

                //do the job with the data here
                //send the data back to client.
                Byte[] sendBytes = Encoding.ASCII.GetBytes("Processed " + data);
                networkStream.Write(sendBytes, 0, sendBytes.Length);
                networkStream.Flush();
            }
            catch (Exception ex)
            {
                throw;
            }

            this.WaitForRequest();
        }
    }

}
