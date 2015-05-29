﻿namespace DraftClient.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using ClientServer;
    using ViewModel;
    using Omu.ValueInjecter;

    public class SetupController
    {
        public bool IsRunning { get; set; }

        public void SubscribeToMessages(ObservableCollection<DraftServer> servers, Client client)
        {
            Dispatcher dispatch = Dispatcher.CurrentDispatcher;
            
            client.ListenForServers(o =>
            {
                var server = new DraftServer();
                server.InjectFrom(o);

                var matchedServer = servers.FirstOrDefault(s => s.IpAddress == server.IpAddress && s.IpPort == server.IpPort);

                if (matchedServer != default(DraftServer))
                {
                    dispatch.Invoke(() => matchedServer.Timeout = DateTime.Now.AddSeconds(7));
                }
                else
                {
                    dispatch.Invoke(() =>
                    {
                        server.Timeout = DateTime.Now.AddSeconds(7);
                        servers.Add(server);
                    });
                }
            });

            Task.Run(() =>
            {
                while (IsRunning)
                {
                    var itemsToRemove = servers.Where(s => s.Timeout < DateTime.Now).ToList();
                    foreach (var item in itemsToRemove)
                    {
                        DraftServer item1 = item;
                        dispatch.Invoke(() => servers.Remove(item1));
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }
}