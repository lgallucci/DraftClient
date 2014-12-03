namespace ClientTest
{
    using ClientServer;
    using System;
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;

    [Serializable]
    public class DraftServer
    {
        public string FantasyDraft { get; set; }
        public int ConnectedPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string ipAddress { get; set; }
        public int ipPort { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.ListenForServers((o) =>
            {
                DraftServer server = new XmlSerializer(typeof(DraftServer)).Deserialize(new MemoryStream(o)) as DraftServer;

                Console.WriteLine("Recived {0} {1}/{2} from {3}:{4}", server.FantasyDraft, server.ConnectedPlayers, server.MaxPlayers, server.ipAddress, server.ipPort);
            });
        }
    }
}
