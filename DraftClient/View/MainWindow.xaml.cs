namespace DraftClient.View
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Omu.ValueInjecter;
    using Controllers;
    using ViewModel;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DraftController _draftController;
        private int _myTeamIndex = -1;
        private bool _dontPrompt;

        public Player DisplayPlayer { get; set; }
        public static PlayerList PlayerList { get; set; }

        public MainWindow(bool isServer)
        {
            InitializeComponent();
            _draftController = new DraftController(this)
            {
                IsServer = isServer
            };

            DisplayPlayer = new Player();
            PlayerList = new PlayerList();

            PlayerView.PlayerClicked += player =>
            {
                DisplayPlayer.InjectFrom(player);
                DisplayPlayer.Schedule = PlayerList.Schedules.First(s => s.Name == DisplayPlayer.Team);
                DisplayPlayer.Histories = PlayerList.Histories.Where(s => s.PlayerId == DisplayPlayer.PlayerId).ToList();

                PlayerFlyout.IsOpen = true;
            };
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (_dontPrompt) return;

            MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Are you sure?{0}This will {1} the draft", Environment.NewLine,
                _draftController.IsServer ? "close" : "leave"), "Close Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                CloseWindow("Draft Closed", false);
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void CloseWindow(string resetMessage, bool closeWindow)
        {
            if (Owner != null)
            {
                ((Setup)Owner).Reset(_draftController.IsServer, resetMessage);
                Owner.Show();
            }

            if (closeWindow)
            {
                _dontPrompt = true;
                Close();
            }
        }

        public async Task<bool> SetupDraft(DraftSettings settings)
        {
            try
            {
                _draftController.Settings = settings;

                LoadPlayers();

                await RetrieveDraft();

                SetupGrid(settings);
            }
            catch (IOException)
            {
                MessageBox.Show("Couldn't find or read the players file.");
                return false;
            }

            return true;
        }

        private async Task RetrieveDraft()
        {
            if (_draftController.IsServer)
            {
                _draftController.CurrentDraft = new Draft(_draftController.Settings.TotalRounds, _draftController.Settings.NumberOfTeams, _draftController.Settings.NumberOfSeconds, true);
            }
            else
            {
                if (!await _draftController.GetDraft())
                {
                    throw new TimeoutException("Didn't recieve draft information in time");
                }
            }
        }

        private void SetupGrid(DraftSettings settings)
        {
            DraftTimerControl.PopulateState(_draftController.CurrentDraft.State);
            DraftTimerControl.DraftStateChanged += state => _draftController.UpdateDraftState(state);

            for (int i = 0; i < settings.NumberOfTeams + 1; i++)
            {
                PicksGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < settings.TotalRounds + 1; i++)
            {
                PicksGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 1; i < PicksGrid.RowDefinitions.Count; i++)
            {
                var roundBlock = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = "ROUND " + i,
                    Name = "Round" + i
                };
                PicksGrid.Children.Add(roundBlock);
                Grid.SetColumn(roundBlock, 0);
                Grid.SetRow(roundBlock, i);
            }

            var myTeam = settings.DraftTeams.FirstOrDefault(d => d.ConnectedUser == _draftController.GetClientId());
            _myTeamIndex = myTeam != null ? myTeam.Index : -1;

            for (int i = 1; i < PicksGrid.ColumnDefinitions.Count; i++)
            {
                var teamBlock = new FantasyTeam
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TeamNumber = i,
                    IsServer = _draftController.IsServer,
                    IsMyTeam = (_myTeamIndex == i),
                };
                teamBlock.TeamChanged += (number, name) => _draftController.UpdateTeam(number, name);
                teamBlock.TeamJoined += number =>
                {
                    _draftController.JoinTeam(number);
                    _myTeamIndex = number;
                    for (int j = 0; j < PicksGrid.ColumnDefinitions.Count; j++)
                    {
                        var teamControl = (FantasyTeam)PicksGrid.Children.Cast<UIElement>().
                            FirstOrDefault(e => Grid.GetColumn(e) == j + 1 && Grid.GetRow(e) == 0);
                        if (teamControl != null)
                        {
                            teamControl.SetConnected(settings.DraftTeams[j].IsConnected, _myTeamIndex > 0);
                            teamControl.IsMyTeam = (_myTeamIndex == j + 1);
                            teamControl.SetText(settings.DraftTeams[j].Name);
                        }
                    }
                };
                teamBlock.SetText(settings.DraftTeams[i - 1].Name);
                teamBlock.SetConnected(settings.DraftTeams[i - 1].IsConnected, _myTeamIndex > 0);
                PicksGrid.Children.Add(teamBlock);
                Grid.SetColumn(teamBlock, i);
                Grid.SetRow(teamBlock, 0);
            }

            for (int i = 1; i < PicksGrid.RowDefinitions.Count; i++)
            {
                for (int j = 1; j < PicksGrid.ColumnDefinitions.Count; j++)
                {
                    var newRound = new FantasyRound
                    {
                        Pick = _draftController.CurrentDraft.Picks[i - 1, j - 1],
                        Round = i,
                        Team = j
                    };
                    if (_draftController.IsServer)
                    {
                        newRound.MakePick += (adp, row, column) => _draftController.MakeMove(new DraftEntities.DraftPick
                        {
                            AverageDraftPosition = adp,
                            Row = row - 1,
                            Column = column - 1
                        });
                    }
                    PicksGrid.Children.Add(newRound);
                    Grid.SetRow(newRound, i);
                    Grid.SetColumn(newRound, j);
                }
            }
        }

        private void LoadPlayers()
        {
            List<DraftEntities.Player> players =
                FileHandler.DraftFileHandler.ReadCsvFile<DraftEntities.Player>("FantasyPlayers.csv");
            List<DraftEntities.PlayerHistory> histories =
                FileHandler.DraftFileHandler.ReadCsvFile<DraftEntities.PlayerHistory>("FantasyPlayersHistory.csv");
            List<DraftEntities.TeamSchedule> schedules =
                FileHandler.DraftFileHandler.ReadCsvFile<DraftEntities.TeamSchedule>("TeamSchedules.csv");

            List<Player> presentationPlayers = players.Select(player =>
            {
                var tempPlayer = new Player();
                tempPlayer.InjectFrom(player);
                return tempPlayer;
            }).ToList();

            List<PlayerHistory> presentationHistories =
                histories.Select(history =>
                {
                    var tempHistory = new PlayerHistory();
                    tempHistory.InjectFrom(history);
                    return tempHistory;
                }).ToList();

            List<TeamSchedule> presentationSchedules = schedules.Select(schedule =>
            {
                var tempSchedule = new TeamSchedule();
                tempSchedule.InjectFrom(schedule);
                return tempSchedule;
            }).ToList();

            PlayerList.Players = new ObservableCollection<Player>(presentationPlayers);
            PlayerList.Histories = new ObservableCollection<PlayerHistory>(presentationHistories);
            PlayerList.Schedules = new ObservableCollection<TeamSchedule>(presentationSchedules);
        }

        public void UpdateTeam(DraftTeam team)
        {
            _draftController.Settings.DraftTeams[team.Index].InjectFrom(team);

            var teamControl = (FantasyTeam)PicksGrid.Children.Cast<UIElement>().
                FirstOrDefault(e => Grid.GetColumn(e) == team.Index + 1 && Grid.GetRow(e) == 0);

            if (teamControl != null)
            {
                if (!string.IsNullOrWhiteSpace(team.Name))
                {
                    teamControl.SetText(team.Name);
                }
                teamControl.SetConnected(team.IsConnected, _myTeamIndex > 0);
            }
        }

        public void UpdateDraftState(DraftEntities.DraftState state)
        {
            DraftTimerControl.UpdateState(state.PickEndTime, state.PickPauseTime, state.Drafting);
        }

        #region PlayerFlyout



        #endregion
    }
}