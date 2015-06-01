namespace DraftClient.View
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using DraftClient.Controllers;
    using DraftClient.ViewModel;
    using DraftEntities;
    using FileReader;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static ViewModel.PlayerList PlayerList = new ViewModel.PlayerList();
        readonly DraftController _draftController;

        public MainWindow(DraftController draftController)
        {
            InitializeComponent();
            _draftController = draftController;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Hide();
            if (Owner != null)
            {
                e.Cancel = true;
                Owner.Show();
            }
        }

        public bool JoinDraft()
        {
            return true;
        }

        public bool SetupDraft(ViewModel.DraftSettings settings)
        {
            try
            {
                _draftController.CurrentDraft = new Draft(settings.TotalRounds, settings.NumberOfTeams, true);
                _draftController.IsServer = true;

                LoadPlayers(settings.PlayerFile);
                SetupGrid(settings);
            }
            catch (IOException)
            {
                MessageBox.Show("Couldn't find or read the players file.  Please enter a valid file location in the box.");
                return false;
            }

            return true;
        }

        private void SetupGrid(ViewModel.DraftSettings settings)
        {

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
                    TeamNumber = i
                };
                teamBlock.SetText("Team " + i);
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
                        Pick = _draftController.CurrentDraft.Picks[i-1,j-1],
                        Round = i,
                        Team = j
                    };
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
                var presentationPlayers = players.Select(player => new PlayerPresentation
                {
                    AverageDraftPosition = player.AverageDraftPosition,
                    Name = player.Name,
                    Position = player.Position,
                    Team = player.Team,
                    ByeWeek = player.ByeWeek,
                    ProjectedPoints = player.ProjectedPoints,
                    IsPicked = false
                }).ToList();

                return new ObservableCollection<PlayerPresentation>(presentationPlayers);
            });
        }
    }
}
