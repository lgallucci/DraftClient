namespace DraftClient.View
{
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
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
            DataContext = Setup.PlayerList;
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
                if (SearchButton.IsChecked.HasValue && SearchButton.IsChecked.Value)
                {
                    e.Accepted = e.Accepted && player.Name.ToLower().Contains(SearchTextBox.Text.ToLower());
                }
                else
                {
                    switch (player.Position)
                    {
                        case PlayerPosition.QB:
                            e.Accepted = cbQB.IsChecked.HasValue && cbQB.IsChecked.Value;
                            break;
                        case PlayerPosition.WR:
                            e.Accepted = cbWR.IsChecked.HasValue && cbWR.IsChecked.Value;
                            break;
                        case PlayerPosition.RB:
                            e.Accepted = cbRB.IsChecked.HasValue && cbRB.IsChecked.Value;
                            break;
                        case PlayerPosition.TE:
                            e.Accepted = cbTE.IsChecked.HasValue && cbTE.IsChecked.Value;
                            break;
                        case PlayerPosition.K:
                            e.Accepted = cbK.IsChecked.HasValue && cbK.IsChecked.Value;
                            break;
                        case PlayerPosition.DEF:
                            e.Accepted = cbDEF.IsChecked.HasValue && cbDEF.IsChecked.Value;
                            break;
                    }
                }
            }
        }

        private void cbPosition_Changed(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource).Refresh();
        }

        private void PlayerClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var viewPlayer = button.DataContext as Player;

                OnPlayerClicked(viewPlayer);
            }
        }

        #region Events

        public delegate void PlayerClickedHandler(Player player);

        public event PlayerClickedHandler PlayerClicked;

        public void OnPlayerClicked(Player player)
        {
            PlayerClickedHandler handler = PlayerClicked;
            if (handler != null)
            {
                handler(player);
            }
        }

        #endregion

        private void FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource).Refresh();
        }

        private async void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (button != null && button.IsChecked != null && !button.IsChecked.Value)
            {
                await Task.Delay(200);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                    {
                        CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource).Refresh();
                    }
                    else
                    {
                        SearchTextBox.Text = "";
                    }
                });
            }
            else
            {
                SearchTextBox.Focus();
            }
        }
    }
}