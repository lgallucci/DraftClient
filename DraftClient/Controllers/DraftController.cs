namespace DraftClient.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using ClientServer;
    using View;
    using Omu.ValueInjecter;
    using DraftEntities;

    public class DraftController
    {
        public bool IsServer { get; set; }
        public ViewModel.Draft CurrentDraft { get; set; }
        public ViewModel.DraftSettings Settings { get; set; }

        private readonly MainWindow _mainWindow;
        private readonly ConnectionServer _connectionServer;

        public DraftController(MainWindow mainWindow)
        {
            _connectionServer = ConnectionServer.Instance;
            _mainWindow = mainWindow;
            _connectionServer.Connection.PickMade += PickMade;
            _connectionServer.Connection.SendDraft += SendDraft;
            _connectionServer.Connection.TeamUpdated += TeamUpdated;
            _connectionServer.Connection.SendDraftSettings += SendDraftSettings;
            _connectionServer.Connection.UserDisconnect += UserDisconnect;
            _connectionServer.Connection.RetrieveDraft += RetrieveDraft;
            _mainWindow.Closed += RemoveHandlers;
        }

        #region Event Handlers

        private void RemoveHandlers(object sender, EventArgs e)
        {
            _connectionServer.Connection.PickMade -= PickMade;
            _connectionServer.Connection.SendDraft -= SendDraft;
            _connectionServer.Connection.TeamUpdated -= TeamUpdated;
            _connectionServer.Connection.SendDraftSettings -= SendDraftSettings;
            _connectionServer.Connection.UserDisconnect -= UserDisconnect;
            _connectionServer.Connection.RetrieveDraft -= RetrieveDraft;
            _mainWindow.Closed -= RemoveHandlers;
        }

        public void GetDraft()
        {
            _connectionServer.Connection.SendMessage(NetworkMessageType.SendDraftMessage, null);


        }

        private void RetrieveDraft(Draft draft)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var draftModel = new ViewModel.Draft(draft.MaxRound, draft.MaxTeam, false);
                draftModel.InjectFrom(draft);
                for (int i = 0; i < draft.MaxRound; i++)
                {
                    for (int j = 0; j < draft.MaxTeam; j++)
                    {
                        if (draft.Picks[i, j] != 0)
                        {
                            var player =
                                MainWindow.PlayerList.Players.First(p => p.AverageDraftPosition == draft.Picks[i, j]);

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
                _mainWindow.SetupDraft(Settings);
            });
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
                            res.Picks[i, j] = src.Picks[i, j].DraftedPlayer.AverageDraftPosition;
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
                foreach (var draftTeam in src.DraftTeams)
                {
                    res.DraftTeams.Add(Mapper.Map<DraftTeam>(draftTeam));
                }
                return res;
            });
            return Mapper.Map<DraftSettings>(Settings);
        }

        private void PickMade(DraftPick pick)
        {
            var player =
                MainWindow.PlayerList.Players.First(p => p.AverageDraftPosition == pick.AverageDraftPosition);

            CurrentDraft.Picks[pick.Row, pick.Column].DraftedPlayer = player;
        }

        private void TeamUpdated(DraftTeam team)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _mainWindow.UpdateTeam(Mapper.Map<ViewModel.DraftTeam>(team));
            });
        }

        private void UserDisconnect(Guid connecteduser)
        {
            var draftTeam = Settings.DraftTeams.FirstOrDefault(d => d.ConnectedUser.Equals(connecteduser));

            if (draftTeam != null)
            {
                draftTeam.IsConnected = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _mainWindow.UpdateTeam(draftTeam);
                });
            }
        }

        #endregion

        public void MakeMove(ViewModel.Player pick)
        {
            _connectionServer.Connection.SendMessage(NetworkMessageType.PickMessage, pick);
        }

        public Guid GetClientId()
        {
            return _connectionServer.Connection.ClientId;
        }
    }
}
