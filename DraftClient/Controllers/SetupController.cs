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

    public class SetupController
    {
        public void SubscribeToMessages(ObservableCollection<ViewModel.DraftServer> Servers, Client client)
        {
            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            client.ListenForServers((o) =>
            {
                var server = new XmlSerializer(typeof(ViewModel.DraftServer)).Deserialize(new MemoryStream(o)) as DraftClient.ViewModel.DraftServer;
                var matchedServer = Servers.FirstOrDefault(s => s.IpAddress == server.IpAddress && s.IpPort == server.IpPort);

                if (matchedServer != default(ViewModel.DraftServer))
                {
                    dispatch.Invoke(() => matchedServer.Timeout = DateTime.Now.AddSeconds(7));
                }
                else
                {
                    dispatch.Invoke(() =>
                    {
                        server.Timeout = DateTime.Now.AddSeconds(7);
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
