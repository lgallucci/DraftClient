using DraftClient.ViewModel;
using System.Windows;

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

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            draftWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
