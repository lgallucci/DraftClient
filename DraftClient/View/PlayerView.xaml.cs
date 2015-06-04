namespace DraftClient.View
{
    using System.Windows;
    using System.Windows.Data;
    using DraftEntities;
    using ViewModel;

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
            PlayerPresentation player = e.Item as PlayerPresentation;
            if (player != null)
            // If filter is turned on, filter completed items.
            {
                switch (player.Position)
                {
                    case PlayerPosition.QB:
                        if (cbQB.IsChecked != null && cbQB.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case PlayerPosition.WR:
                        if (cbWR.IsChecked != null && cbWR.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case PlayerPosition.RB:
                        if (cbRB.IsChecked != null && cbRB.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case PlayerPosition.TE:
                        if (cbTE.IsChecked != null && cbTE.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case PlayerPosition.K:
                        if (cbK.IsChecked != null && cbK.IsChecked.Value) { e.Accepted = true; }
                        else { e.Accepted = false; }
                        break;
                    case PlayerPosition.DEF:
                        if (cbDEF.IsChecked != null && cbDEF.IsChecked.Value) { e.Accepted = true; }
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
