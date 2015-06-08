﻿namespace DraftClient.Controllers
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
    using Omu.ValueInjecter;
    using DraftEntities;

    public class SetupController
    {
        public bool IsRunning { get; set; }

        private readonly Setup _setupWindow;
        private readonly ConnectionServer _connectionServer;
        public bool IsConnected { get { return _connectionServer.Connection.IsConnected; } }

        public SetupController(Setup setupWindow)
        {
            _connectionServer = ConnectionServer.Instance;
            _setupWindow = setupWindow;
            _connectionServer.Connection.RetrieveDraftSettings += RetrieveDraftSettings;
        }

        public void SubscribeToMessages(ObservableCollection<ViewModel.DraftServer> servers)
        {
            Dispatcher dispatch = Application.Current.Dispatcher;

            _connectionServer.Connection.ListenForServers(o =>
            {
                var server = new ViewModel.DraftServer();
                server.InjectFrom(o);

                var matchedServer = servers.FirstOrDefault(s => s.IpAddress == server.IpAddress && s.IpPort == server.IpPort);

                if (matchedServer != default(ViewModel.DraftServer))
                {
                    dispatch.Invoke(() =>
                    {
                        matchedServer.InjectFrom(server);
                        matchedServer.Timeout = DateTime.Now.AddSeconds(7);
                    });
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

        private void RetrieveDraftSettings(DraftSettings settings)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _setupWindow.DraftSettings.InjectFrom(settings);
                _setupWindow.DraftSettings.DraftTeams = new ObservableCollection<ViewModel.DraftTeam>();
                foreach (var draftTeam in settings.DraftTeams)
                {
                    _setupWindow.DraftSettings.DraftTeams.Add(Mapper.Map<ViewModel.DraftTeam>(draftTeam));
                }
                _setupWindow.SettingsResetEvent.Set();
                _setupWindow.SelectTeam(false);
            });
        }

        public void ConnectToDraftServer(string ipAddress, int ipPort)
        {
            _connectionServer.ConnectToDraft(ipAddress, ipPort);
        }

        public void CancelDraft()
        {
            
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

        public Guid GetClientId()
        {
            return _connectionServer.Connection.ClientId;
        }
    }
}
