using DraftClient.ViewModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace DraftClient.View
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : UserControl
    {
        public PlayerView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DataContext = MainWindow.PlayerList;
            this.cbQB.Checked += cbPosition_Changed; this.cbQB.Unchecked += cbPosition_Changed;
            this.cbWR.Checked += cbPosition_Changed; this.cbWR.Unchecked += cbPosition_Changed;
            this.cbRB.Checked += cbPosition_Changed; this.cbRB.Unchecked += cbPosition_Changed;
            this.cbTE.Checked += cbPosition_Changed; this.cbTE.Unchecked += cbPosition_Changed;
            this.cbK.Checked += cbPosition_Changed; this.cbK.Unchecked += cbPosition_Changed;
            this.cbDEF.Checked += cbPosition_Changed; this.cbDEF.Unchecked += cbPosition_Changed;
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            PlayerPresentation player = e.Item as PlayerPresentation;
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

        private void cbPosition_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(this.dataGrid1.ItemsSource).Refresh();
        }
    }
}
