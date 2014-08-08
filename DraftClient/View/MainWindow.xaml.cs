using DraftClient.View;
using DraftClient.ViewModel;
using DraftEntities;
using FileReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DraftClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PlayerList playerList = new PlayerList();

        public MainWindow()
        {
            InitializeComponent();
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

        public bool SetupDraft(DraftSettings settings)
        {
            SetupGrid(settings);
            try
            {
                LoadPlayers(settings.PlayerFile);
            }catch (IOException)
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
                    FontFamily = new System.Windows.Media.FontFamily("Franklin Gothic Medium"),
                    FontSize = 16
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
                    HorizontalAlignment = HorizontalAlignment.Center
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
                }
            }
        }

        private void LoadPlayers(string playerFile)
        {
            List<Player> players = DraftFileHandler.ReadFile(playerFile);
            var presentationPlayers = new List<PlayerPresentation>();

            foreach (Player player in players)
            {
                presentationPlayers.Add(new PlayerPresentation
                {
                    ADP = player.ADP,
                    Name = player.Name,
                    Position = player.Position,
                    Team = player.Team,
                    ByeWeek = player.ByeWeek,
                    YahooADP = player.YahooADP,
                    ESPNADP = player.ESPNADP,
                    CBSADP = player.CBSADP,
                    ProjectedPoints = player.ProjectedPoints,
                    IsPicked = false
                });
            }

            playerList.Players = new ObservableCollection<PlayerPresentation>(presentationPlayers);
        }

    }
}
