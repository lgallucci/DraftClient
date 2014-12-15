namespace DraftEntities.View
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
    using DraftEntities;
    using FileReader;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static PlayerList PlayerList = new PlayerList();
        readonly DraftController _draftController;

        public MainWindow(DraftController draftController)
        {
            InitializeComponent();
            _draftController = draftController;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            this.Hide();
            if (this.Owner != null)
            {
                e.Cancel = true;
                this.Owner.Show();
            }
        }

        public bool JoinDraft()
        {
            _draftController.OnPickMade += PickMade;

            return true;
        }

        public void PickMade(PickEventArgs e)
        {
            PlayerPresentation pick = PlayerList.Players.FirstOrDefault(p => p.AverageDraftPosition == e.AverageDraftPosition);

        }

        public bool SetupDraft(DraftSettings settings)
        {
            try
            {
                LoAverageDraftPositionlayers(settings.PlayerFile);
                SetupGrid(settings);
                _draftController.IsServer = true;
            }
            catch (IOException)
            {
                MessageBox.Show("Couldn't find or read the players file.  Please enter a valid file location in the box.");
                return false;
            }

            return true;
        }

        private void SetupGrid(DraftSettings settings)
        {
            var numberOfRounds = settings.Quarterbacks + settings.WideRecievers + settings.RunningBacks +
                settings.FlexWithTightEnd + settings.FlexWithoutTightEnd + settings.TightEnds +
                settings.Kickers + settings.Defenses + settings.BenchPlayers;

            for (int i = 0; i < settings.NumberOfTeams + 1; i++)
            {
                this.PicksGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < numberOfRounds + 1; i++)
            {
                this.PicksGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 1; i < this.PicksGrid.RowDefinitions.Count; i++)
            {
                var roundBlock = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = "Round " + i,
                    Name = "Round" + i
                };
                this.PicksGrid.Children.Add(roundBlock);
                Grid.SetColumn(roundBlock, 0);
                Grid.SetRow(roundBlock, i);
            }

            for (int i = 1; i < this.PicksGrid.ColumnDefinitions.Count; i++)
            {
                var teamBlock = new FantasyTeam()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TeamNumber = i
                };
                teamBlock.SetText("Team " + i);
                this.PicksGrid.Children.Add(teamBlock);
                Grid.SetColumn(teamBlock, i);
                Grid.SetRow(teamBlock, 0);
            }

            for (int i = 1; i < this.PicksGrid.RowDefinitions.Count; i++)
            {
                for (int j = 1; j < this.PicksGrid.ColumnDefinitions.Count; j++)
                {
                    var newRound = new FantasyRound();
                    this.PicksGrid.Children.Add(newRound);
                    Grid.SetRow(newRound, i);
                    Grid.SetColumn(newRound, j);
                    newRound.Round = i;
                    newRound.Team = j;
                }
            }
        }

        private async void LoAverageDraftPositionlayers(string playerFile)
        {
            List<Player> players = DraftFileHandler.ReadFile(playerFile);

            PlayerList.Players = await Task.Run(() =>
            {
                var presentationPlayers = new List<PlayerPresentation>();

                foreach (Player player in players)
                {
                    presentationPlayers.Add(new PlayerPresentation
                    {
                        AverageDraftPosition = player.AverageDraftPosition,
                        Name = player.Name,
                        Position = player.Position,
                        Team = player.Team,
                        ByeWeek = player.ByeWeek,
                        ProjectedPoints = player.ProjectedPoints,
                        IsPicked = false
                    });
                }
                return new ObservableCollection<PlayerPresentation>(presentationPlayers);
            });
        }
    }
}
