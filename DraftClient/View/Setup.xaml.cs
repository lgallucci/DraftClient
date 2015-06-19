namespace DraftClient.View
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using DraftClient.Controllers;
    using DraftClient.Extensions;
    using DraftClient.ViewModel;
    using MahApps.Metro;
    using MahApps.Metro.Controls;

    /// <summary>
    ///     Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup
        //TODO: Create Theme chooser
    {
        private readonly SetupController _setupController;
        private MainWindow _draftWindow;

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
        public SpinnyWindow ConnectingWindow = new SpinnyWindow();

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {
            ConnectingWindow.ShowWithText(string.Format("Starting {0}...", DraftSettings.LeagueName));
            OpenDraft(true);
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
                    _draftWindow.Title = string.Format("Fantasy Draft - {0}", DraftSettings.LeagueName);
                    _draftWindow.Show();
                    ConnectingWindow.Hide();
                }
            }
            catch (TimeoutException ex)
            {
                MessageBox.Show(ex.Message);
                _setupController.DisconnectFromDraftServer();
                ConnectingWindow.Hide();
            }
        }

        public void OpenDraft(bool isServer)
        {
            if (!isServer && !_setupController.IsConnected)
            {
                throw new Exception("Didn't successfully connect to the draft");
            }

            if (isServer) _setupController.StartServer(DraftSettings.LeagueName, DraftSettings.NumberOfTeams);

            CreateDraftWindow(isServer, DraftSettings.TotalRounds, DraftSettings.NumberOfTeams).DoNotAwait();
        }

        private void CreateDraft_Click(object sender, RoutedEventArgs e)
        {
            ServerSetupViewer.Visibility = Visibility.Visible;
            ThemeViewer.Visibility = Visibility.Collapsed;
            StartupViewer.Visibility = Visibility.Collapsed;
            Title = "Create Draft";
        }

        private async void JoinDraft_Click(object sender, RoutedEventArgs e) 
        {
            var lbi = ServerListBox.SelectedItem as DraftServer;
            if (lbi != null)
            {
                try
                {
                    ConnectingWindow.ShowWithText(string.Format("Connecting to {0}...", lbi.FantasyDraft));
                    _setupController.ConnectToDraftServer(lbi.IpAddress, lbi.IpPort);

                    if (!await _setupController.GetDraftSettings())
                    {
                        throw new TimeoutException("Didn't recieve draft settings");
                    }
                    OpenDraft(false);
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

        public void Reset(bool isServer)
        {
            if (!isServer)
                _setupController.DisconnectFromDraftServer();
            _setupController.ResetConnection();
            DraftSettings.Reset();
        }

        private void ThemeButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerSetupViewer.Visibility = Visibility.Collapsed;
            ThemeViewer.Visibility = Visibility.Visible;
            StartupViewer.Visibility = Visibility.Collapsed;
            Title = "Choose Theme";
        }

        private void CancelDraft_Click(object sender, RoutedEventArgs e)
        {
            ServerSetupViewer.Visibility = Visibility.Collapsed;
            ThemeViewer.Visibility = Visibility.Collapsed;
            StartupViewer.Visibility = Visibility.Visible;
            Title = "Join Draft";
        }

        private void ChangeTheme_OnClick(object sender, RoutedEventArgs e)
        {
            var themeName = ((DockPanel)((Button)sender).Parent.GetParentObject()).Tag.ToString();
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var themeAccent = ThemeManager.GetAccent(themeName);
            ThemeManager.ChangeAppStyle(Application.Current, themeAccent, theme.Item1);

            ServerSetupViewer.Visibility = Visibility.Collapsed;
            ThemeViewer.Visibility = Visibility.Collapsed;
            StartupViewer.Visibility = Visibility.Visible;
            Title = "Join Draft";
        }

        private void ChangeBaseTheme_OnClick(object sender, RoutedEventArgs e)
        {
            var themeName = ((DockPanel)((Button)sender).Parent.GetParentObject()).Tag.ToString();
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var themeApp = ThemeManager.GetAppTheme(themeName);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, themeApp);

            ServerSetupViewer.Visibility = Visibility.Collapsed;
            ThemeViewer.Visibility = Visibility.Visible;
            ThemeLightDarkViewer.Visibility = Visibility.Collapsed;
            StartupViewer.Visibility = Visibility.Collapsed;
        }
    }
}
