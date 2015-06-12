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
    using DraftClient.Controllers;
    using DraftEntities;
    using FileReader;
    using Omu.ValueInjecter;
    using Draft = DraftClient.ViewModel.Draft;
    using DraftSettings = DraftClient.ViewModel.DraftSettings;
    using DraftTeam = DraftClient.ViewModel.DraftTeam;
    using Player = DraftEntities.Player;
    using PlayerList = DraftClient.ViewModel.PlayerList;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static PlayerList PlayerList = new PlayerList();
        private readonly DraftController _draftController;
        public AutoResetEvent DraftReset;

        public MainWindow(bool isServer)
        {
            InitializeComponent();
            _draftController = new DraftController(this)
            {
                IsServer = isServer
            };
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Are you sure?{0}This will {1} the draft", Environment.NewLine,
                _draftController.IsServer ? "close" : "leave"), "Close Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (Owner != null)
                {
                    ((Setup) Owner).Reset();
                    Owner.Show();
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        public async Task<bool> SetupDraft(DraftSettings settings)
        {
            try
            {
                _draftController.Settings = settings;

                LoadPlayers(settings.PlayerFile);

                await RetrieveDraft();

                SetupGrid(settings);
            }
            catch (IOException)
            {
                MessageBox.Show("Couldn't find or read the players file.  Please enter a valid file location in the box.");
                return false;
            }

            return true;
        }

        private async Task RetrieveDraft()
        {
            if (_draftController.IsServer)
            {
                _draftController.CurrentDraft = new Draft(_draftController.Settings.TotalRounds, _draftController.Settings.NumberOfTeams, true);
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
                    Text = "Round " + i,
                    Name = "Round" + i
                };
                PicksGrid.Children.Add(roundBlock);
                Grid.SetColumn(roundBlock, 0);
                Grid.SetRow(roundBlock, i);
            }

            for (int i = 1; i < PicksGrid.ColumnDefinitions.Count; i++)
            {
                var teamBlock = new FantasyTeam
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TeamNumber = i,
                    IsServer = _draftController.IsServer,
                    IsMyTeam = (settings.DraftTeams[i - 1].ConnectedUser == _draftController.GetClientId()),
                };
                teamBlock.TeamChanged += (number, name) => _draftController.UpdateTeam(number, name);
                teamBlock.SetText(settings.DraftTeams[i - 1].Name);
                teamBlock.SetConnected(settings.DraftTeams[i - 1].IsConnected);
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
                        newRound.MakePick += (adp, row, column) => _draftController.MakeMove(new DraftPick
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

        private async void LoadPlayers(string playerFile)
        {
            List<Player> players = DraftFileHandler.ReadFile(playerFile);

            PlayerList.Players = await Task.Run(() =>
            {
                List<ViewModel.Player> presentationPlayers = players.Select(player => new ViewModel.Player
                {
                    AverageDraftPosition = player.AverageDraftPosition,
                    Name = player.Name,
                    Position = player.Position,
                    Team = player.Team,
                    ByeWeek = player.ByeWeek,
                    ProjectedPoints = player.ProjectedPoints,
                    IsPicked = false
                }).ToList();

                return new ObservableCollection<ViewModel.Player>(presentationPlayers);
            });
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
                teamControl.SetConnected(team.IsConnected);
            }
        }
    }
}