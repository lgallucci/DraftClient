﻿namespace DraftClient.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using ClientServer;
    using DraftClient.View;
    using DraftClient.ViewModel;
    using Omu.ValueInjecter;
    using DraftSettings = DraftEntities.DraftSettings;

    public class SetupController
    {
        private readonly ConnectionServer _connectionServer;
        private readonly Setup _setupWindow;
        private AutoResetEvent _settingsResetEvent;

        public SetupController(Setup setupWindow)
        {
            _connectionServer = ConnectionServer.Instance;
            _setupWindow = setupWindow;
            _connectionServer.RetrieveDraftSettings += RetrieveDraftSettings;
            _connectionServer.TeamUpdated += TeamUpdated;
        }

        private void TeamUpdated(DraftEntities.DraftTeam team)
        {
            _setupWindow.DraftSettings.DraftTeams[team.Index].InjectFrom(team);
        }

        public bool IsRunning { get; set; }

        public bool IsConnected
        {
            get { return _connectionServer.IsConnected; }
        }

        public void SubscribeToMessages(ObservableCollection<DraftServer> servers)
        {
            Dispatcher dispatch = Application.Current.Dispatcher;

            _connectionServer.ListenForServers(o =>
            {
                var server = new DraftServer();
                server.InjectFrom(o);

                DraftServer matchedServer = servers.FirstOrDefault(s => s.IpAddress == server.IpAddress && s.IpPort == server.IpPort);

                if (matchedServer != default(DraftServer))
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
                    List<DraftServer> itemsToRemove = servers.Where(s => s.Timeout < DateTime.Now).ToList();
                    foreach (DraftServer item in itemsToRemove)
                    {
                        DraftServer item1 = item;
                        dispatch.Invoke(() => servers.Remove(item1));
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public Task<bool> GetDraftSettings()
        {
            _settingsResetEvent = new AutoResetEvent(false);
            _connectionServer.SendMessage(NetworkMessageType.SendDraftSettingsMessage, null);

            return Task.Run(() => _settingsResetEvent.WaitOne(5000));
        }

        private void RetrieveDraftSettings(DraftSettings settings)
        {
            if (settings == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                _setupWindow.DraftSettings.InjectFrom(settings);
                _setupWindow.DraftSettings.DraftTeams = new ObservableCollection<DraftTeam>();
                foreach (DraftEntities.DraftTeam draftTeam in settings.DraftTeams)
                {
                    _setupWindow.DraftSettings.DraftTeams.Add(Mapper.Map<DraftTeam>(draftTeam));
                }
                
                _setupWindow.DraftSettings.CurrentDraft = new Draft(settings.CurrentDraft.MaxRound,
                    settings.CurrentDraft.MaxTeam, settings.NumberOfSeconds, false);
                
                for (int i = 0; i < settings.CurrentDraft.MaxRound; i++)
                {
                    for (int j = 0; j < settings.CurrentDraft.MaxTeam; j++)
                    {
                        if (settings.CurrentDraft.Picks[i, j] != 0)
                        {
                            ViewModel.Player player =
                                Setup.PlayerList.Players.First(p => p.Rank == settings.CurrentDraft.Picks[i, j]);

                            _setupWindow.DraftSettings.CurrentDraft.Picks[i][j] = new ViewModel.DraftPick
                            {
                                DraftedPlayer = player,
                                CanEdit = false,
                                Name = player.Name
                            };
                        }
                    }
                }
            });
            _settingsResetEvent.Set();
        }

        public void ConnectToDraftServer(string ipAddress, int ipPort)
        {
            _connectionServer.ConnectToDraft(ipAddress, ipPort);
        }

        public void CancelDraft()
        {
            _connectionServer.ResetConnection();
        }

        public void StartServer(string leagueName, int numberOfTeams)
        {
            _connectionServer.StartServer(leagueName, numberOfTeams);
        }

        public void DisconnectFromDraftServer()
        {
            _connectionServer.SendMessage(NetworkMessageType.LogoutMessage, null);
        }

        public Guid GetClientId()
        {
            return _connectionServer.GetClientId();
        }

        public void ResetConnection()
        {
            _connectionServer.ResetConnection();
        }

        public void LoadDraft(string draftName)
        {
            var settings =
                FileHandler.DraftFileHandler.ReadFile<DraftSettings>(string.Format("DRAFT_{0}.xml", draftName));
            
            _setupWindow.DraftSettings.InjectFrom(settings);
            _setupWindow.DraftSettings.DraftTeams = new ObservableCollection<DraftTeam>();
            foreach (DraftEntities.DraftTeam draftTeam in settings.DraftTeams)
            {
                _setupWindow.DraftSettings.DraftTeams.Add(Mapper.Map<DraftTeam>(draftTeam));
            }

            _setupWindow.DraftSettings.CurrentDraft = new Draft(settings.CurrentDraft.MaxRound,
                settings.CurrentDraft.MaxTeam, settings.NumberOfSeconds, true);

            for (int i = 0; i < settings.CurrentDraft.MaxRound; i++)
            {
                for (int j = 0; j < settings.CurrentDraft.MaxTeam; j++)
                {
                    if (settings.CurrentDraft.Picks[i, j] != 0)
                    {
                        ViewModel.Player player =
                            Setup.PlayerList.Players.First(p => p.Rank == settings.CurrentDraft.Picks[i, j]);

                        _setupWindow.DraftSettings.CurrentDraft.Picks[i][j] = new ViewModel.DraftPick
                        {
                            DraftedPlayer = player,
                            CanEdit = true,
                            Name = player.Name
                        };
                    }
                }
            }
        }

        public string[] LoadPreviousDrafts()
        {
            return GetDraftNames(FileHandler.DraftFileHandler.GetFilesWithPrefix("DRAFT"));
        }

        private string[] GetDraftNames(string[] filesWithPath)
        {
            var fileNames = new string[filesWithPath.Length];
            for(int i = 0; i < filesWithPath.Length; i++)
            {
                fileNames[i] = Path.GetFileName(filesWithPath[i]).Replace("DRAFT_", "").Replace(".xml", "");
            }
            return fileNames;
        }
    }
}