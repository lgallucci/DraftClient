﻿namespace DraftClient.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ClientServer;
    using DraftClient.View;
    using DraftEntities;
    using Omu.ValueInjecter;

    public class DraftController
    {
        private readonly ConnectionServer _connectionServer;
        private readonly MainWindow _mainWindow;

        public DraftController(MainWindow mainWindow)
        {
            _connectionServer = ConnectionServer.Instance;
            _mainWindow = mainWindow;
            _connectionServer.PickMade += PickMade;
            _connectionServer.SendDraft += SendDraft;
            _connectionServer.TeamUpdated += TeamUpdated;
            _connectionServer.SendDraftSettings += SendDraftSettings;
            _connectionServer.UserDisconnect += UserDisconnect;
            _connectionServer.RetrieveDraft += RetrieveDraft;
            _connectionServer.DraftStop += DraftStop;
            _connectionServer.DraftStateChanged += DraftStateChanged;
            _mainWindow.Closed += RemoveHandlers;
        }

        #region Event Handlers

        private void RemoveHandlers(object sender, EventArgs e)
        {
            _connectionServer.PickMade -= PickMade;
            _connectionServer.SendDraft -= SendDraft;
            _connectionServer.TeamUpdated -= TeamUpdated;
            _connectionServer.SendDraftSettings -= SendDraftSettings;
            _connectionServer.UserDisconnect -= UserDisconnect;
            _connectionServer.RetrieveDraft -= RetrieveDraft;
            _connectionServer.DraftStop -= DraftStop;
            _connectionServer.DraftStateChanged -= DraftStateChanged;
            _mainWindow.Closed -= RemoveHandlers;
        }

        public Task<bool> GetDraft()
        {
            _draftReset = new AutoResetEvent(false);
            _connectionServer.SendMessage(NetworkMessageType.SendDraftMessage, null);

            return Task.Run(() => _draftReset.WaitOne(5000));
        }

        private void RetrieveDraft(Draft draft)
        {
            if (draft == null) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var draftModel = new ViewModel.Draft(draft.MaxRound, draft.MaxTeam, Settings.NumberOfSeconds, false);
                draftModel.InjectFrom(draft);
                for (int i = 0; i < draft.MaxRound; i++)
                {
                    for (int j = 0; j < draft.MaxTeam; j++)
                    {
                        if (draft.Picks[i, j] != 0)
                        {
                            ViewModel.Player player =
                                MainWindow.PlayerList.Players.First(p => p.Rank == draft.Picks[i, j]);

                            draftModel.Picks[i, j] = new ViewModel.DraftPick
                            {
                                DraftedPlayer = player,
                                CanEdit = IsServer,
                                Name = player.Name
                            };
                        }
                    }
                }
                CurrentDraft = draftModel;
            });

            _draftReset.Set();
        }

        private Draft SendDraft()
        {
            Mapper.AddMap<ViewModel.Draft, Draft>(src =>
            {
                var res = new Draft();
                res.InjectFrom(src);
                int rows = src.Picks.GetLength(0),
                    columns = src.Picks.GetLength(1);

                res.Picks = new int[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (src.Picks[i, j].DraftedPlayer != null)
                        {
                            res.Picks[i, j] = src.Picks[i, j].DraftedPlayer.Rank;
                        }
                    }
                }
                return res;
            });
            return Mapper.Map<Draft>(CurrentDraft);
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
                return res;
            });
            return Mapper.Map<DraftSettings>(Settings);
        }

        private void PickMade(DraftPick pick)
        {
            ViewModel.Player player =
                MainWindow.PlayerList.Players.FirstOrDefault(p => p.Rank == pick.Rank);

            CurrentDraft.Picks[pick.Row, pick.Column].DraftedPlayer = player;
            CurrentDraft.Picks[pick.Row, pick.Column].Name = (player != null) ? player.Name : "";
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
        public ViewModel.Draft CurrentDraft { get; set; }
        public ViewModel.DraftSettings Settings { get; set; }
        private AutoResetEvent _draftReset;

        public void MakeMove(DraftPick pick)
        {
            _connectionServer.SendMessage(NetworkMessageType.PickMessage, pick);
        }

        public Guid GetClientId()
        {
            return _connectionServer.GetClientId();
        }

        public void UpdateTeam(int teamNumber, string name)
        {
            var team = Settings.DraftTeams[teamNumber-1];

            team.Name = name;

            _connectionServer.SendMessage(NetworkMessageType.UpdateTeamMessage, Mapper.Map<DraftTeam>(team));
        }

        public void JoinTeam(int teamNumber)
        {
            var team = Settings.DraftTeams[teamNumber-1];
            team.ConnectedUser = GetClientId();
            team.IsConnected = true;
            _connectionServer.SendMessage(NetworkMessageType.UpdateTeamMessage, Mapper.Map<DraftTeam>(team));
        }

        public void UpdateDraftState(ViewModel.DraftState state)
        {
            _connectionServer.SendMessage(NetworkMessageType.UpdateDraftState, Mapper.Map<DraftState>(state));
        }

        public void DraftStateChanged(DraftState state)
        {
            Application.Current.Dispatcher.Invoke(() => _mainWindow.UpdateDraftState(state));
        }

        public void DraftStop()
        {
            Application.Current.Dispatcher.Invoke(() => _mainWindow.CloseWindow("Draft Closed by Server", true));
        }
    }
}