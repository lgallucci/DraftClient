using DraftClient.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DraftClient.View
{
    /// <summary>
    /// Interaction logic for FantasyRound.xaml
    /// </summary>
    public partial class FantasyRound : UserControl
    {
        public FantasyRound()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new DraftPick();
        }
    }
}
