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

        public bool IsRunning { get; set; }

        public Client()
        {
            IsRunning = true;
        }

        public void ListenForServers(Action<byte[]> ServerPingCallback)
        {            
            Task.Run(() =>
            {
                UdpClient client = new UdpClient();

                client.ExclusiveAddressUse = false;
                IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 11000);

                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.ExclusiveAddressUse = false;

                client.Client.Bind(localEp);

                IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
                client.JoinMulticastGroup(multicastaddress);
                
                while (true)
                {
                    var ServerBroadcastData = client.Receive(ref localEp);

                    ServerPingCallback(ServerBroadcastData);
                }
            });
        }
    }
}
