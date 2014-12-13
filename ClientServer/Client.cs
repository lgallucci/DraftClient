namespace ClientServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class Client
    {
        //Listen for Draft Messages

        //Send Messages on Draft Picks
        public Task ServerListener;
        private UdpClient _client;
        protected bool IsRunning;

        public Client()
        {
            IsRunning = true;
        }

        public void ListenForServers(Action<byte[]> serverPingCallback)
        {
            ServerListener = Task.Run(() =>
            {
                _client = new UdpClient();

                _client.ExclusiveAddressUse = false;
                IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 11000);

                _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _client.ExclusiveAddressUse = false;

                _client.Client.Bind(localEp);

                IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
                _client.JoinMulticastGroup(multicastaddress);

                while (IsRunning)
                {
                    var serverBroadcastData = _client.Receive(ref localEp);

                    serverPingCallback(serverBroadcastData);
                }
            });
        }

        public void ConnectToDraftServer(string ipAdress, string port)
        {

        }

        public void ReceiveDraftServerMessage()
        {

        }

        public void Dispose()
        {
            IsRunning = false;
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            if (ServerListener != null)
            {                
                ServerListener.Dispose();
            }
        }
    }
}
