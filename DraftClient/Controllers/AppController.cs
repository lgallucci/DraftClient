namespace DraftClient.Controllers
{
    using ClientServer;
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using System.Xml.Serialization;

    public class AppController
    {
        public void SubscribeToMessages(ObservableCollection<DraftClient.ViewModel.DraftServer> Servers)
        {
            Client _client = new Client();

            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            _client.ListenForServers((o) =>
            {
                DraftClient.ViewModel.DraftServer server = new XmlSerializer(typeof(DraftClient.ViewModel.DraftServer)).Deserialize(new MemoryStream(o)) as DraftClient.ViewModel.DraftServer;
                var matchedServer = Servers.FirstOrDefault(s => s.ipAddress == server.ipAddress && s.ipPort == server.ipPort);

                if (matchedServer != default(ViewModel.DraftServer))
                {
                    dispatch.Invoke(() => matchedServer.Timeout = DateTime.Now.AddSeconds(10));
                }
                else
                {
                    dispatch.Invoke(() =>
                    {
                        server.Timeout = DateTime.Now.AddSeconds(10);
                        Servers.Add(server);
                    });
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    var itemsToRemove = Servers.Where(s => s.Timeout < DateTime.Now).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        dispatch.Invoke(() => Servers.Remove(item));
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
