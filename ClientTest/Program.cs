namespace ClientTest
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using ClientServer;
    using DraftEntities;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            var runner = new ClientRunner();
            runner.RunClient();
        }
    }

    internal class ClientRunner
    {
        private Client _client;
        private AutoResetEvent _reset;

        public void RunClient()
        {
            var servers = new Collection<DraftServer>();

            _client = new Client();
            _client.ListenForServers(server =>
            {
                if (server != null)
                {
                    DraftServer match = servers.FirstOrDefault(s => s.IpPort == server.IpPort && s.IpAddress == server.IpAddress);
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
                foreach (DraftServer server in servers)
                {
                    Console.WriteLine("Server: {0} {1}/{2} from {3}:{4}", server.FantasyDraft, server.ConnectedPlayers,
                        server.MaxPlayers, server.IpAddress, server.IpPort);
                }

                try
                {
                    Console.WriteLine("Please enter your name within the next 5 seconds.");
                    string name = Reader.ReadLine(5000);

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        _reset = new AutoResetEvent(false);

                        _client.ConnectToDraftServer(servers[0].IpAddress, servers[0].IpPort);
                        _client.ServerHandshake += ServerHandshake;
                        _client.SendMessage(NetworkMessageType.LoginMessage, Guid.NewGuid().ToString());

                        _reset.WaitOne(5000);

                        Console.WriteLine("Connected to {0} as {1}", servers[0].FantasyDraft, name);

                        _client.RetrieveDraft += RetrieveDraft;
                        _client.RetrieveDraftSettings += RetrieveDraftSettings;

                        while (true)
                        {
                            GetDraftSettings();
                            Thread.Sleep(5000);
                            GetDraft();
                            Thread.Sleep(5000);
                        }
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Sorry, you waited too long.");
                }
            }
        }

        private void ServerHandshake()
        {
            Console.WriteLine("Recieved Login Handshake");
            _reset.Set();
        }

        private void GetDraftSettings()
        {
            Console.WriteLine("Getting DraftSettings!");
            _client.SendMessage(NetworkMessageType.SendDraftSettingsMessage, null);
        }

        private void RetrieveDraftSettings(DraftSettings settings)
        {
            Console.WriteLine("Recieved DraftSettings!");
        }

        private void GetDraft()
        {
            Console.WriteLine("Getting Draft!");
            _client.SendMessage(NetworkMessageType.SendDraftMessage, null);
        }

        private void RetrieveDraft(Draft draft)
        {
            Console.WriteLine("Recieved Draft!");
        }
    }
}