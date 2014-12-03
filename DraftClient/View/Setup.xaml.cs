namespace DraftClient.View
{
    using DraftClient.Controllers;
    using DraftClient.ViewModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        MainWindow draftWindow;
        AppController _appController;
        DraftSettings _draftSettings;

        public Setup()
        {
            InitializeComponent();

            _draftSettings = new DraftSettings();
            _appController = new AppController();

            this.DataContext = _draftSettings;

            _appController.SubscribeToMessages(_draftSettings.Servers);
            ServerListBox.ItemsSource = _draftSettings.Servers;
        }

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {
            draftWindow = new MainWindow(_appController);
            if (draftWindow.SetupDraft(this.DataContext as DraftSettings))
            {
                draftWindow.Owner = this;
                this.Hide();
                draftWindow.Show();
                ContinueButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ContinueDraft_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            draftWindow.Show();
        }

        private void CreateDraft_Click(object sender, RoutedEventArgs e)
        {
            Startup_Viewer.Visibility = System.Windows.Visibility.Collapsed;
            ServerSetup_Viewer.Visibility = System.Windows.Visibility.Visible;
        }

        private void CancelDraft_Click(object sender, RoutedEventArgs e)
        {
            Startup_Viewer.Visibility = System.Windows.Visibility.Visible;
            ServerSetup_Viewer.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void JoinDraft_Click(object sender, RoutedEventArgs e)
        {
            DraftServer lbi = ServerListBox.SelectedItem as DraftServer;
            MessageBox.Show(string.Format("{0} {1}/{2} {3}:{4}", lbi.FantasyDraft, lbi.ConnectedPlayers, lbi.MaxPlayers, lbi.ipAddress, lbi.ipPort));
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
