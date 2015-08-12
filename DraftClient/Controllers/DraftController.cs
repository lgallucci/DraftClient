namespace DraftClient.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using ClientServer;
    using DraftClient.View;
    using DraftEntities;
    using Omu.ValueInjecter;
    using Providers;
    using MahApps.Metro.Controls.Dialogs;
    using System.Threading.Tasks;
    using System.Threading;

    public class DraftController
    {
        private readonly ConnectionService _connectionService;
        private readonly MainWindow _mainWindow;
        private AutoResetEvent _settingsResetEvent;

        public DraftController(MainWindow mainWindow)
        {
            _connectionService = ConnectionService.Instance;
            _mainWindow = mainWindow;
            _connectionService.PickMade += PickMade;
            _connectionService.TeamUpdated += TeamUpdated;
            _connectionService.SendDraftSettings += SendDraftSettings;
            _connectionService.UserDisconnect += UserDisconnect;
            _connectionService.DraftStop += DraftStop;
            _connectionService.DraftStateChanged += DraftStateChanged;
            _connectionService.Disconnect += DraftDisconnected;
            _mainWindow.Closed += RemoveHandlers;
        }

        #region Event Handlers

        private void RemoveHandlers(object sender, EventArgs e)
        {
            _connectionService.PickMade -= PickMade;
            _connectionService.TeamUpdated -= TeamUpdated;
            _connectionService.SendDraftSettings -= SendDraftSettings;
            _connectionService.UserDisconnect -= UserDisconnect;
            _connectionService.DraftStop -= DraftStop;
            _connectionService.DraftStateChanged -= DraftStateChanged;
            _connectionService.Disconnect -= DraftDisconnected;
            _mainWindow.Closed -= RemoveHandlers;
        }

        private DraftSettings SendDraftSettings()
        {
            Mapper.AddMap<ViewModel.DraftSettings, DraftSettings>(src =>
            {
                var res = new DraftSettings();
                res.InjectFrom(src);
                res.DraftTeams = new Collection<DraftTeam>();
                foreach (ViewModel.DraftTeam draftTeam in src.DraftTeams)
                {
                    res.DraftTeams.Add(Mapper.Map<DraftTeam>(draftTeam));
                }
                int rows = src.CurrentDraft.Picks.Count,
                    columns = src.CurrentDraft.Picks[0].Count;

                res.CurrentDraft = new Draft { Picks = new int[rows, columns] };
                res.CurrentDraft.InjectFrom(src.CurrentDraft);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (src.CurrentDraft.Picks[i][j].DraftedPlayer != null)
                        {
                            res.CurrentDraft.Picks[i, j] = src.CurrentDraft.Picks[i][j].DraftedPlayer.Rank;
                        }
                    }
                }
                return res;
            });
            return Mapper.Map<DraftSettings>(Settings);
        }

        private void PickMade(DraftPick pick)
        {
            ViewModel.Player player =
                Globals.PlayerList.Players.FirstOrDefault(p => p.Rank == pick.Rank);

            Settings.CurrentDraft.Picks[pick.Row][pick.Column].DraftedPlayer = player;
            Settings.CurrentDraft.Picks[pick.Row][pick.Column].Name = (player != null) ? player.Name : "";
        }

        private void TeamUpdated(DraftTeam team)
        {
            Application.Current.Dispatcher.Invoke(() => _mainWindow.UpdateTeam(Mapper.Map<ViewModel.DraftTeam>(team)));
        }

        private void UserDisconnect(Guid connecteduser)
        {
            ViewModel.DraftTeam draftTeam = Settings.DraftTeams.FirstOrDefault(d => d.ConnectedUser.Equals(connecteduser));

            if (draftTeam != null)
            {
                draftTeam.IsConnected = false;
                Application.Current.Dispatcher.Invoke(() => _mainWindow.UpdateTeam(draftTeam));
            }
        }

        #endregion

        public bool IsServer { get; set; }
        public ViewModel.DraftSettings Settings { get; set; }

        public void MakeMove(DraftPick pick)
        {
            _connectionService.SendMessage(NetworkMessageType.PickMessage, pick);
        }

        public Guid GetClientId()
        {
            return _connectionService.GetClientId();
        }

        public void UpdateTeam(int teamNumber, string name)
        {
            var team = Settings.DraftTeams[teamNumber - 1];

            team.Name = name;

            _connectionService.SendMessage(NetworkMessageType.UpdateTeamMessage, Mapper.Map<DraftTeam>(team));
        }

        public void JoinTeam(int teamNumber)
        {
            var team = Settings.DraftTeams[teamNumber - 1];
            team.ConnectedUser = GetClientId();
            team.IsConnected = true;
            _connectionService.SendMessage(NetworkMessageType.UpdateTeamMessage, Mapper.Map<DraftTeam>(team));
        }

        public void UpdateDraftState(ViewModel.DraftState state)
        {
            _connectionService.SendMessage(NetworkMessageType.UpdateDraftState, Mapper.Map<DraftState>(state));
        }

        public void DraftStateChanged(DraftState state)
        {
            Application.Current.Dispatcher.Invoke(() => _mainWindow.UpdateDraftState(state));
        }

        public void DraftStop()
        {
            Application.Current.Dispatcher.Invoke(() => _mainWindow.CloseWindow("Draft Closed by Server"));
        }

        public void SaveDraft()
        {
            var settings = new DraftSettings();
            settings.InjectFrom(Settings);
            settings.DraftTeams = new Collection<DraftTeam>();
            foreach (ViewModel.DraftTeam draftTeam in Settings.DraftTeams)
            {
                draftTeam.IsConnected = false;
                draftTeam.ConnectedUser = Guid.Empty;
                settings.DraftTeams.Add(Mapper.Map<DraftTeam>(draftTeam));
            }
            int rows = Settings.CurrentDraft.Picks.Count,
                columns = Settings.CurrentDraft.Picks[0].Count;

            settings.CurrentDraft = new Draft { Picks = new int[rows, columns] };
            settings.CurrentDraft.InjectFrom(Settings.CurrentDraft);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (Settings.CurrentDraft.Picks[i][j].DraftedPlayer != null)
                    {
                        settings.CurrentDraft.Picks[i, j] = Settings.CurrentDraft.Picks[i][j].DraftedPlayer.Rank;
                    }
                }
            }

            FileHandler.DraftFileHandler.WriteFile(settings, string.Format("DRAFT_{0}", Settings.LeagueName));
        }
        
        private void DraftDisconnected()
        {            
            _connectionService.ResetConnection();
            RemoveHandlers(this, null);

            Application.Current.Dispatcher.Invoke(async () => 
            {
                //var controller = await _mainWindow.ShowReconnectingDialog();

                //var timeOut = DateTime.Now.AddMinutes(1);

                //while (!controller.IsCanceled && timeOut > DateTime.Now)
                //{
                //    await Task.Delay(500);

                //    try
                //    {
                //        if (await _connectionService.ConnectToDraft())
                //        {
                //            break;
                //        }
                //    }
                //    catch (TimeoutException) { }
                //}

                bool hasReconnected = false;

                //if (_connectionService.IsConnected)
                //{
                //    controller.SetMessage("Getting Updated Draft...");
                //    hasReconnected = await _mainWindow.ReloadDraft();
                //}

                //await controller.CloseAsync();

                if (!hasReconnected)
                {
                    _mainWindow.CloseWindow("Lost connection.  Please reconnect.");
                }
            });
        }
    }
}