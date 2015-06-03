namespace DraftClient.View
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using ClientServer;
    using DraftClient.Controllers;
    using DraftClient.ViewModel;

    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup
    {
        MainWindow _draftWindow;
        readonly DraftSettings _draftSettings;
        DraftController _draftController;
        private readonly SetupController _setupController;
        Client _client;

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

            _draftController = new DraftController(_client, _draftWindow)
            {
                IsServer = true
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
                //TODO: Send Connect As Team Event
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
            GetDraftSettings();
            SelectTeam(false);

            LoadingIndicatorJoin.Visibility = Visibility.Visible;
            var lbi = ServerListBox.SelectedItem as DraftServer;
            if (lbi != null)
            {
                _client.ConnectToDraftServer(lbi.IpAddress, lbi.IpPort);
                MessageBox.Show(string.Format("{0} {1}/{2} {3}:{4}", lbi.FantasyDraft, lbi.ConnectedPlayers, lbi.MaxPlayers, lbi.IpAddress, lbi.IpPort));
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
            if (lbi != null)
            {
                JoinDraftButton.IsEnabled = true;
            }
            else
            {
                JoinDraftButton.IsEnabled = false;
            }
        }
    }
}
