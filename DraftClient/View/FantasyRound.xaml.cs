namespace DraftClient.View
{
    using System.Windows;
    using DraftClient.ViewModel;

    /// <summary>
    ///     Interaction logic for FantasyRound.xaml
    /// </summary>
    public partial class FantasyRound
    {
        public FantasyRound()
        {
            InitializeComponent();
        }

        public int Round { get; set; }
        public int Team { get; set; }
        public DraftPick Pick { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Pick;
        }
    }
}