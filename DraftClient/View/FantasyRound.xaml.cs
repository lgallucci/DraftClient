using DraftClient.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace DraftClient.View
{
    /// <summary>
    /// Interaction logic for FantasyRound.xaml
    /// </summary>
    public partial class FantasyRound : UserControl
    {
        public int Round { get; set; }
        public int Team { get; set; }

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
