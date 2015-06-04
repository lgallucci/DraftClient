namespace DraftClient.View
{
    using DraftClient.ViewModel;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = MainWindow.PlayerList;
            cbQB.Checked += cbPosition_Changed; cbQB.Unchecked += cbPosition_Changed;
            cbWR.Checked += cbPosition_Changed; cbWR.Unchecked += cbPosition_Changed;
            cbRB.Checked += cbPosition_Changed; cbRB.Unchecked += cbPosition_Changed;
            cbTE.Checked += cbPosition_Changed; cbTE.Unchecked += cbPosition_Changed;
            cbK.Checked += cbPosition_Changed; cbK.Unchecked += cbPosition_Changed;
            cbDEF.Checked += cbPosition_Changed; cbDEF.Unchecked += cbPosition_Changed;
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            Player player = e.Item as Player;
            if (player != null)
            // If filter is turned on, filter completed items.
            {
                switch (player.Position)
                {
                    case DraftEntities.PlayerPosition.QB:
                        if (cbQB.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case DraftEntities.PlayerPosition.WR:
                        if (cbWR.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case DraftEntities.PlayerPosition.RB:
                        if (cbRB.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case DraftEntities.PlayerPosition.TE:
                        if (cbTE.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case DraftEntities.PlayerPosition.K:
                        if (cbK.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case DraftEntities.PlayerPosition.DEF:
                        if (cbDEF.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                }
            }
        }

        private void cbPosition_Changed(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource).Refresh();
        }
    }
}
