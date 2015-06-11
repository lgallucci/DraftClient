namespace DraftClient.View
{
    using System.Windows;
    using System.Windows.Data;
    using DraftEntities;
    using Player = DraftClient.ViewModel.Player;

    /// <summary>
    ///     Interaction logic for PlayerView.xaml
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
            cbQB.Checked += cbPosition_Changed;
            cbQB.Unchecked += cbPosition_Changed;
            cbWR.Checked += cbPosition_Changed;
            cbWR.Unchecked += cbPosition_Changed;
            cbRB.Checked += cbPosition_Changed;
            cbRB.Unchecked += cbPosition_Changed;
            cbTE.Checked += cbPosition_Changed;
            cbTE.Unchecked += cbPosition_Changed;
            cbK.Checked += cbPosition_Changed;
            cbK.Unchecked += cbPosition_Changed;
            cbDEF.Checked += cbPosition_Changed;
            cbDEF.Unchecked += cbPosition_Changed;
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var player = e.Item as Player;
            if (player != null)
                // If filter is turned on, filter completed items.
            {
                switch (player.Position)
                {
                    case PlayerPosition.QB:
                        e.Accepted = cbQB.IsChecked.Value;
                        break;
                    case PlayerPosition.WR:
                        e.Accepted = cbWR.IsChecked.Value;
                        break;
                    case PlayerPosition.RB:
                        e.Accepted = cbRB.IsChecked.Value;
                        break;
                    case PlayerPosition.TE:
                        e.Accepted = cbTE.IsChecked.Value;
                        break;
                    case PlayerPosition.K:
                        e.Accepted = cbK.IsChecked.Value;
                        break;
                    case PlayerPosition.DEF:
                        e.Accepted = cbDEF.IsChecked.Value;
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