namespace DraftClient.View
{
    using ClientServer;
    using DraftClient.Controllers;
    using DraftClient.ViewModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;

    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        MainWindow draftWindow;
        SetupController _setupController;
        DraftSettings _draftSettings;
        DraftController _draftController;
        Client _client;

        public Setup()
        {
            InitializeComponent();

            _client = new Client();
            _draftSettings = new DraftSettings();
            _setupController = new SetupController();

            _setupController.SubscribeToMessages(_draftSettings.Servers, _client);

            this.DataContext = _draftSettings;
        }

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {
            LoadingIndicatorCreate.Visibility = Visibility.Visible;

            _client = new Server(_draftSettings.LeagueName, _draftSettings.NumberOfTeams);
            ((Server)_client).StartServer();

            _draftController = new DraftController(_client)
            {
                IsServer = true
            };

            draftWindow = new MainWindow(_draftController);
            if (draftWindow.SetupDraft(this.DataContext as DraftSettings))
            {
                draftWindow.Owner = this;
                this.Hide();
                draftWindow.Show();
                StartButton.Visibility = Visibility.Collapsed;
                ContinueButton.Visibility = Visibility.Visible;
                LoadingIndicatorCreate.Visibility = Visibility.Collapsed;
            }
        }

        private void ContinueDraft_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            draftWindow.Show();
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
            if (_client is Server)
            {
                ((Server)_client).StopServer();
            }

            Startup_Viewer.Visibility = Visibility.Visible;
            ServerSetup_Viewer.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Visible;
            ContinueButton.Visibility = Visibility.Collapsed;
        }

        private void JoinDraft_Click(object sender, RoutedEventArgs e)
        {
            LoadingIndicatorJoin.Visibility = Visibility.Visible;
            DraftServer lbi = ServerListBox.SelectedItem as DraftServer;
            MessageBox.Show(string.Format("{0} {1}/{2} {3}:{4}", lbi.FantasyDraft, lbi.ConnectedPlayers, lbi.MaxPlayers, lbi.IpAddress, lbi.IpPort));
            LoadingIndicatorJoin.Visibility = Visibility.Collapsed;
        }

        private void ServerBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetDraftButtonEnabled();
        }

        private void DraftName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetDraftButtonEnabled();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SetDraftButtonEnabled()
        {
            var lbi = ServerListBox.SelectedItem;
            if (lbi != null && DraftName.Text.Length > 0)
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
