namespace ClientTest
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using ClientServer;
    using DraftEntities;

    class Program
    {
        static void Main(string[] args)
        {
            var servers = new Collection<DraftServer>();

            var client = new Client();
            client.ListenForServers((server) =>
            {
                if (server != null)
                {
                    var match = servers.FirstOrDefault(s => s.IpPort == server.IpPort && s.IpAddress == server.IpAddress);
                    if (match != null)
                    {
                        servers[servers.IndexOf(match)] = server;
                    }
                    else
                    {
                        servers.Add(server);
                    }
                }

            });

            while (true)
            {
                Console.Clear();
                foreach (var server in servers)
                {
                    Console.WriteLine("Server: {0} {1}/{2} from {3}:{4}", server.FantasyDraft, server.ConnectedPlayers, server.MaxPlayers, server.IpAddress, server.IpPort);
                }

                try
                {
                    Console.WriteLine("Please enter your name within the next 5 seconds.");
                    string name = Reader.ReadLine(5000);

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var tcpClient = new TcpClient();
                        tcpClient.Connect(new IPEndPoint(IPAddress.Parse(servers[0].IpAddress), servers[0].IpPort));

                        using (var memoryStream = new MemoryStream())
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(memoryStream, new NetworkMessage
                            {
                                MessageType = NetworkMessageType.LoginMessage,
                                MessageContent = name
                            });
                            tcpClient.Client.Send(memoryStream.ToArray());
                            Console.WriteLine("Connected to {0} as {1}", servers[0].FantasyDraft, name);
                        }

                        while (true)
                        {

                        }
                    }

                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Sorry, you waited too long.");
                }
            }

        }
    }
}
