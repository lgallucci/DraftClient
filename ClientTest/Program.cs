namespace ClientTest
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using ClientServer;
    using DraftClient.Controllers;
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
        private ConnectionServer _connection;
        private AutoResetEvent _reset;

        private bool recievedDraft = false;
        private bool recievedDraftSettings = false;

        public void RunClient()
        {
            var servers = new Collection<DraftServer>();

            _connection = ConnectionServer.Instance;
            _connection.ListenForServers(server =>
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

                        _connection.ConnectToDraft(servers[0].IpAddress, servers[0].IpPort);
                        _connection.SendMessage(NetworkMessageType.LoginMessage, Guid.NewGuid().ToString());

                        _reset.WaitOne(5000);

                        Console.WriteLine("Connected to {0} as {1}", servers[0].FantasyDraft, name);

                        _connection.RetrieveDraft += RetrieveDraft;
                        _connection.RetrieveDraftSettings += RetrieveDraftSettings;

                        while (true)
                        {
                            Thread.Sleep(5000);
                            recievedDraftSettings = false;
                            recievedDraft = false;
                            GetDraftSettings();
                            Thread.Sleep(5000);
                            GetDraft();
                            Thread.Sleep(5000); 

                            while (!recievedDraftSettings || !recievedDraft)
                            {
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Sorry, you waited too long.");
                }
            }
        }

        private void GetDraftSettings()
        {
            Console.WriteLine("Getting DraftSettings!");
            _connection.SendMessage(NetworkMessageType.SendDraftSettingsMessage, null);
        }

        private void RetrieveDraftSettings(DraftSettings settings)
        {
            recievedDraftSettings = true;
            Console.WriteLine("Recieved DraftSettings!");
        }

        private void GetDraft()
        {
            Console.WriteLine("Getting Draft!");
            _connection.SendMessage(NetworkMessageType.SendDraftMessage, null);
        }

        private void RetrieveDraft(Draft draft)
        {
            recievedDraft = true;
            Console.WriteLine("Recieved Draft!");
        }
    }
}