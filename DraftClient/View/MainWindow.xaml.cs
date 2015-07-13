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
    using System.Windows.Media;
    using MahApps.Metro.Controls.Dialogs;
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

        const int screenwidth = 1600;
        const int screenheight = 900;

        public MainWindow(bool isServer)
        {
            InitializeComponent();
            _draftController = new DraftController(this)
            {
                IsServer = isServer
            };

            DisplayPlayer = new Player();

            PlayerView.PlayerClicked += player =>
            {
                DisplayPlayer.InjectFrom(player);
                DisplayPlayer.Schedule = Setup.PlayerList.Schedules.First(s => s.Name == DisplayPlayer.Team);
                DisplayPlayer.Histories = Setup.PlayerList.Histories.Where(s => s.PlayerId == DisplayPlayer.PlayerId).ToList();

                PlayerFlyout.IsOpen = true;
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var width = SystemParameters.PrimaryScreenWidth;
            var height = SystemParameters.PrimaryScreenHeight;


            double scalex = width / screenwidth;
            double scaley = height / screenheight;

            MainGrid.LayoutTransform = new ScaleTransform(scalex, scaley);
            PlayerFlyout.LayoutTransform = new ScaleTransform(scalex, scaley);
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            if (_dontPrompt) return;

            e.Cancel = true;

            MessageDialogResult messageBoxResult = await this.ShowMessageAsync(string.Format("{0} the draft?", _draftController.IsServer ? "Close" : "Leave"), "Are you sure?"
                , MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });

            if (messageBoxResult == MessageDialogResult.Affirmative)
            {
                CloseWindow("Draft Closed");
            }
        }

        public void CloseWindow(string resetMessage)
        {
            if (_draftController.IsServer)
            {
                _draftController.SaveDraft();
            }

            if (Owner != null)
            {
                ((Setup)Owner).Reset(_draftController.IsServer, resetMessage);
                Owner.Show();
            }

            _dontPrompt = true;
            Close();
        }

        public bool SetupDraft(DraftSettings settings)
        {
            _draftController.Settings = settings;

            if (_draftController.IsServer)
            {
                if (_draftController.Settings.CurrentDraft == null)
                {
                    _draftController.Settings.CurrentDraft = new Draft(_draftController.Settings.TotalRounds,
                        _draftController.Settings.NumberOfTeams, _draftController.Settings.NumberOfSeconds, true);
                }
            }

            SetupGrid(settings);

            return true;
        }

        private void SetupGrid(DraftSettings settings)
        {
            DraftTimerControl.PopulateState(_draftController.Settings.CurrentDraft.State);
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
                teamBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                teamBlock.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                teamBlock.VerticalAlignment = VerticalAlignment.Stretch;
            }

            for (int i = 1; i < PicksGrid.RowDefinitions.Count; i++)
            {
                for (int j = 1; j < PicksGrid.ColumnDefinitions.Count; j++)
                {
                    var newRound = new FantasyRound
                    {
                        Pick = _draftController.Settings.CurrentDraft.Picks[i - 1][j - 1],
                        Round = i,
                        Team = j
                    };
                    if (_draftController.IsServer)
                    {
                        newRound.MakePick += (adp, row, column) => _draftController.MakeMove(new DraftEntities.DraftPick
                        {
                            Rank = adp,
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