using DraftClient.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DraftClient.View
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        MainWindow draftWindow;
        public Setup()
        {
            InitializeComponent();
            this.DataContext = new DraftSettings();
        }

        private void StartDraft_Click(object sender, RoutedEventArgs e)
        {
            draftWindow = new MainWindow();
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
            MessageBox.Show(string.Format("{0} {1}/{2} {3}:{4}", lbi.FantasyDraft,lbi.ConnectedPlayers, lbi.MaxPlayers, lbi.tcpAddress.Address, lbi.tcpAddress.Port));
        }

        private void ServerBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            if (lbi != null && lbi.Content.ToString() != "...")
            {
                JoinDraftButton.IsEnabled = true;
            }
            else
            {
                JoinDraftButton.IsEnabled = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
