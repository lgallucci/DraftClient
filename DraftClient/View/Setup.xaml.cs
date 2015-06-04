﻿namespace DraftClient.View
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using ClientServer;
    using Controllers;
    using ViewModel;

    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup
    {
        private MainWindow _draftWindow;
        private readonly DraftSettings _draftSettings;
        private DraftController _draftController;
        private readonly SetupController _setupController;
        private Client _client;

        public Setup()
        {
            InitializeComponent();

            _client = new Client();
            _draftSettings = new DraftSettings();
            _setupController = new SetupController
            {
                IsRunning = true
            };

            _setupController.SubscribeToMessages(_draftSettings.Servers, _client);

            DataContext = _draftSettings;
        }

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {
            SelectTeam(true);

            LoadingIndicatorCreate.Visibility = Visibility.Visible;

            _client = new Server(_draftSettings.LeagueName, _draftSettings.NumberOfTeams);
            ((Server)_client).StartServer();

            CreateDraftWindow(true);
        }

        private void CreateDraftWindow(bool isServer)
        {
            _draftController = new DraftController(_client, _draftWindow)
            {
                IsServer = isServer
            };

            _draftWindow = new MainWindow(_draftController);
            if (_draftWindow.SetupDraft(DataContext as DraftSettings))
            {
                _draftWindow.Owner = this;
                Hide();
                _draftWindow.Show();
                StartButton.Visibility = Visibility.Collapsed;
                ContinueButton.Visibility = Visibility.Visible;
                LoadingIndicatorCreate.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectTeam(bool isServer)
        {
            var teamSelect = new TeamSelect
            {
                IsServer = isServer,
                Teams = _draftSettings.DraftTeams
            };

            teamSelect.ShowDialog();

            _draftSettings.MyTeamIndex = teamSelect.Team.Index;

            if (!isServer)
            {
                
            }

        }

        private void ContinueDraft_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            _draftWindow.Show();
        }

        private void CreateDraft_Click(object sender, RoutedEventArgs e)
        {
            Startup_Viewer.Visibility = Visibility.Collapsed;
            ServerSetup_Viewer.Visibility = Visibility.Visible;
            LoadingIndicatorCreate.Visibility = Visibility.Collapsed;
        }

        private void CancelDraft_Click(object sender, RoutedEventArgs e)
        {
            _draftSettings.Servers.Remove(_draftSettings.Servers.FirstOrDefault(s => s.FantasyDraft == _draftSettings.LeagueName));

            _client.Close();

            Startup_Viewer.Visibility = Visibility.Visible;
            ServerSetup_Viewer.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            ContinueButton.Visibility = Visibility.Collapsed;
        }

        private void JoinDraft_Click(object sender, RoutedEventArgs e)
        {
            LoadingIndicatorJoin.Visibility = Visibility.Visible;

            _client = new Client();
            var lbi = ServerListBox.SelectedItem as DraftServer;
            if (lbi != null)
            {
                _client.ConnectToDraftServer(lbi.IpAddress, lbi.IpPort);

                GetDraftSettings();
                SelectTeam(false);

                CreateDraftWindow(false);
            }
            LoadingIndicatorJoin.Visibility = Visibility.Collapsed;
        }

        private void GetDraftSettings()
        {
            //TODO: Get Draft Settings!
        }

        private void ServerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDraftButtonEnabled();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _setupController.IsRunning = false;
            _draftController = null;
            Application.Current.Shutdown();
        }

        private void SetDraftButtonEnabled()
        {
            var lbi = ServerListBox.SelectedItem;
            JoinDraftButton.IsEnabled = lbi != null;
        }
    }
}
