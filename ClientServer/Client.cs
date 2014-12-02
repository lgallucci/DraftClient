namespace ClientServer
{
    using System.Data;
    using System.Net;
    using System.Net.Sockets;
    using System;
    using System.Text;

    public class Client
    {
        //Listen for Draft Messages

        //Send Messages on Draft Picks

        public Client()
        {

        }

        public void DoWork()
        {
            var Client = new UdpClient(8888);

            while (true)
            {
                var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                var ServerBroadcastData = Client.Receive(ref ServerEp);
                var ServerBroadcast = Encoding.ASCII.GetString(ServerBroadcastData);

                Console.WriteLine("Recived {0} from {1}:{2}", ServerBroadcast, ServerEp.Address, ServerEp.Port);
            }
        }
    }
}
