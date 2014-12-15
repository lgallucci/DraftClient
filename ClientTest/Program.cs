namespace ClientTest
{
    using ClientServer;
    using System;
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;
    using DraftEntities;

    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();
            client.ListenForServers((o) =>
            {
                DraftServer server = new XmlSerializer(typeof(DraftServer)).Deserialize(new MemoryStream(o)) as DraftServer;

                Console.WriteLine("Recived {0} {1}/{2} from {3}:{4}", server.FantasyDraft, server.ConnectedPlayers, server.MaxPlayers, server.IpAddress, server.IpPort);
            });
        }
    }
}
