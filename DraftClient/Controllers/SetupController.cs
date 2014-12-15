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
        public void SubscribeToMessages(ObservableCollection<ViewModel.DraftServer> servers, Client client)
        {
            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            client.ListenForServers((o) =>
            {
                var server = new XmlSerializer(typeof(ViewModel.DraftServer)).Deserialize(new MemoryStream(o)) as ViewModel.DraftServer;
                var matchedServer = servers.FirstOrDefault(s => server != null && (s.IpAddress == server.IpAddress && s.IpPort == server.IpPort));

                if (matchedServer != default(ViewModel.DraftServer))
                {
                    dispatch.Invoke(() => matchedServer.Timeout = DateTime.Now.AddSeconds(7));
                }
                else
                {
                    dispatch.Invoke(() =>
                    {
                        if (server != null)
                        {
                            server.Timeout = DateTime.Now.AddSeconds(7);
                            servers.Add(server);
                        }
                    });
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    var itemsToRemove = servers.Where(s => s.Timeout < DateTime.Now).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        dispatch.Invoke(() => servers.Remove(item));
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
