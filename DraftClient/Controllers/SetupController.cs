namespace DraftClient.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using ClientServer;
    using DraftClient.View;
    using DraftEntities;
    using Omu.ValueInjecter;

    public class SetupController
    {
        public bool IsRunning { get; set; }
        
        private readonly Setup _setupWindow;
        private readonly ConnectionServer _connectionServer;

        public SetupController(Setup setupWindow)
        {
            _connectionServer = ConnectionServer.Instance;
            _setupWindow = setupWindow;
            _connectionServer.Connection.RetrieveDraft += RetrieveDraft;
            _connectionServer.Connection.RetrieveDraftSettings += RetrieveDraftSettings;
        }
        public void SubscribeToMessages(ObservableCollection<ViewModel.DraftServer> servers)
        {
            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            _connectionServer.Connection.ListenForServers(o =>
            {
                var server = new ViewModel.DraftServer();
                server.InjectFrom(o);

                var matchedServer = servers.FirstOrDefault(s => s.IpAddress == server.IpAddress && s.IpPort == server.IpPort);

                if (matchedServer != default(ViewModel.DraftServer))
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
                        ViewModel.DraftServer item1 = item;
                        dispatch.Invoke(() => servers.Remove(item1));
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void GetDraftSettings()
        {
            _connectionServer.Connection.SendMessage(NetworkMessageType.SendDraftSettingsMessage, null);
        }

        public void GetDraft()
        {
            _connectionServer.Connection.SendMessage(NetworkMessageType.SendDraftMessage, null);
        }

        private void RetrieveDraft(Draft draft)
        {
            Application.Current.Dispatcher.Invoke(() => _setupWindow.CreateDraftWindow(false, Mapper.Map<ViewModel.Draft>(draft)));
        }

        private void RetrieveDraftSettings(DraftSettings settings)
        {
            Application.Current.Dispatcher.Invoke(() => _setupWindow.SelectTeam(false, (ViewModel.DraftSettings) _setupWindow.DraftSettings.InjectFrom(settings)));
        }

        public void ConnectToDraftServer(string ipAddress, int ipPort)
        {
            _connectionServer.ConnectToDraft(ipAddress, ipPort);
        }

        public void CancelDraft()
        {
            _connectionServer.Connection.Close();
        }

        public void StartServer(string leagueName, int numberOfTeams)
        {
            _connectionServer.StartServer(leagueName, numberOfTeams);
        }

        public void UpdateTeamInfo(ViewModel.DraftTeam team)
        {
            team.ConnectedUser = _connectionServer.Connection.ClientId;
            _connectionServer.Connection.SendMessage(NetworkMessageType.UpdateTeamMessage, Mapper.Map<ViewModel.DraftTeam, DraftTeam>(team));   
        }

        public void DisconnectFromDraftServer()
        {
            _connectionServer.Connection.SendMessage(NetworkMessageType.LogoutMessage, null);
        }
    }
}
