namespace DraftClient.View
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Controllers;
    using ViewModel;

    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup
    {
        private MainWindow _draftWindow;
        private readonly SetupController _setupController;

        public DraftSettings DraftSettings { get; set; }

        public Setup()
        {
            InitializeComponent();

            DraftSettings = new DraftSettings();
            _setupController = new SetupController(this)
            {
                IsRunning = true
            };

            _setupController.SubscribeToMessages(DraftSettings.Servers);

            DataContext = DraftSettings;
        }

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {
            LoadingIndicatorCreate.Visibility = Visibility.Visible;

            SelectTeam(true);
        }

        public void CreateDraftWindow(bool isServer, Draft draft)
        {
            _draftWindow = new MainWindow(isServer, draft);

            if (_draftWindow.SetupDraft(DraftSettings))
            {
                _draftWindow.Owner = this;
                Hide();
                _draftWindow.Show();
                StartButton.Visibility = Visibility.Collapsed;
                ContinueButton.Visibility = Visibility.Visible;
                LoadingIndicatorCreate.Visibility = Visibility.Collapsed;
            }
        }

        public void SelectTeam(bool isServer)
        {
            var teamSelect = new TeamSelect
            {
                IsServer = isServer,
                Teams = DraftSettings.DraftTeams
            };

            teamSelect.ShowDialog();
            
            if (teamSelect.DialogResult == true)
            {
                if (isServer)
                {
                    teamSelect.Team.ConnectedUser = _setupController.GetClientId();
                    _setupController.StartServer(DraftSettings.LeagueName, DraftSettings.NumberOfTeams);
                    CreateDraftWindow(true, new Draft(DraftSettings.TotalRounds, DraftSettings.NumberOfTeams, true));
                }
                else
                {
                    _setupController.UpdateTeamInfo(teamSelect.Team);
                    _setupController.GetDraft();
                }
            }
            else
            {
                _setupController.DisconnectFromDraftServer();
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
            DraftSettings.Servers.Remove(DraftSettings.Servers.FirstOrDefault(s => s.FantasyDraft == DraftSettings.LeagueName));

            _setupController.CancelDraft();

            Startup_Viewer.Visibility = Visibility.Visible;
            ServerSetup_Viewer.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            ContinueButton.Visibility = Visibility.Collapsed;
        }

        private void JoinDraft_Click(object sender, RoutedEventArgs e)
        {
            LoadingIndicatorJoin.Visibility = Visibility.Visible;

            var lbi = ServerListBox.SelectedItem as DraftServer;
            if (lbi != null)
            {
                try
                {
                    _setupController.ConnectToDraftServer(lbi.IpAddress, lbi.IpPort);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    _setupController.DisconnectFromDraftServer();
                }
                _setupController.GetDraftSettings();
            }
        }

        private void ServerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDraftButtonEnabled();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _setupController.IsRunning = false;
            _setupController.DisconnectFromDraftServer();
            Application.Current.Shutdown();
        }

        private void SetDraftButtonEnabled()
        {
            var lbi = ServerListBox.SelectedItem;
            JoinDraftButton.IsEnabled = lbi != null;
        }
    }
}
