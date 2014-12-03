namespace ClientServer
{
    using System.Net;
    using System.Net.Sockets;
    using System;
    using System.Text;

    public class Client
    {
        //Listen for Draft Messages

        //Send Messages on Draft Picks

        public bool IsRunning { get; set; }

        public Client()
        {
            IsRunning = true;
        }

        public void ListenForServers()
        {
            var Client = new UdpClient(8888);

            while (IsRunning)
            {
                var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                var ServerBroadcastData = Client.Receive(ref ServerEp);
                var ServerBroadcast = Encoding.ASCII.GetString(ServerBroadcastData);

                Console.WriteLine("Recived {0} from {1}:{2}", ServerBroadcast, ServerEp.Address, ServerEp.Port);
            }
        }
    }
}
