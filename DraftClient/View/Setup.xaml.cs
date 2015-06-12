namespace DraftClient.View
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using DraftClient.Controllers;
    using DraftClient.ViewModel;

    /// <summary>
    ///     Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup
    {
        private readonly SetupController _setupController;
        private MainWindow _draftWindow;
        //TODO: Allow Client to resume draft
        public Setup()
        {
            InitializeComponent();

            DraftSettings = DraftSettings.Instance;
            _setupController = new SetupController(this)
            {
                IsRunning = true
            };

            _setupController.SubscribeToMessages(DraftSettings.Servers);

            DataContext = DraftSettings;
        }

        public DraftSettings DraftSettings { get; set; }

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {

            SelectTeam(true);
        }

        public async Task CreateDraftWindow(bool isServer, int totalRounds, int numberOfTeams)
        {
            try
            {
                _draftWindow = new MainWindow(isServer);

                if (await _draftWindow.SetupDraft(DraftSettings))
                {
                    _draftWindow.Owner = this;
                    Hide();
                    _draftWindow.Show();
                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.Message);
                _setupController.DisconnectFromDraftServer();
            }
        }

        public void SelectTeam(bool isServer)
        {
            if (!isServer && !_setupController.IsConnected)
            {
                return;
            }

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
                    CreateDraftWindow(true, DraftSettings.TotalRounds, DraftSettings.NumberOfTeams);
                }
                else
                {
                    _setupController.UpdateTeamInfo(teamSelect.Team);
                    CreateDraftWindow(false, DraftSettings.TotalRounds, DraftSettings.NumberOfTeams);
                }
            }
            else
            {
                _setupController.DisconnectFromDraftServer();
            }
        }

        private void CreateDraft_Click(object sender, RoutedEventArgs e)
        {
            Startup_Viewer.Visibility = Visibility.Collapsed;
            ServerSetup_Viewer.Visibility = Visibility.Visible;
        }

        private async void JoinDraft_Click(object sender, RoutedEventArgs e) // TODO: Better streamline connecting to server to make it less Jarring (WPF Toolkit Busy Indicator ?
        {
            var lbi = ServerListBox.SelectedItem as DraftServer;
            if (lbi != null)
            {
                try
                {
                    _setupController.ConnectToDraftServer(lbi.IpAddress, lbi.IpPort);
                    
                    if (!await _setupController.GetDraftSettings())
                    {
                        throw new TimeoutException("Didn't recieve draft settings");
                    }
                    SelectTeam(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    _setupController.DisconnectFromDraftServer();
                }
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
            object lbi = ServerListBox.SelectedItem;
            JoinDraftButton.IsEnabled = lbi != null;
        }

        public void Reset()
        {
            //TODO: Reset all variables!
        }
    }
}
