namespace DraftClient.View
{
    using System.Windows;
    using DraftClient.ViewModel;

    /// <summary>
    ///     Interaction logic for FantasyRound.xaml
    /// </summary>
    public partial class FantasyRound //TODO: Update autocomplete to use theme colors
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
            if (Pick.DraftedPlayer != null)
            {
                PlayerAutoComplete.ItemsSelector.Items.Add(Pick.DraftedPlayer);
                PlayerAutoComplete.ItemsSelector.SelectedItem = Pick.DraftedPlayer;
            }
            Pick.MakePick += adp => OnMakePick(adp, Round, Team);
        }

        #region Events

        public delegate void MakePickHandler(int adp, int row, int column);

        public event MakePickHandler MakePick;

        public void OnMakePick(int adp, int row, int column)
        {
            MakePickHandler handler = MakePick;
            if (handler != null)
            {
                handler(adp, row, column);
            }
        }

        #endregion
    }
}