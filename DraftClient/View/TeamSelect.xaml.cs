

namespace DraftClient.View
{
    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;
    using DraftClient.ViewModel;

    /// <summary>
    /// Interaction logic for TeamSelect.xaml
    /// </summary>
    public partial class TeamSelect : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public TeamSelect()
        {
            InitializeComponent();
        }

        public bool IsServer { get; set; }
        public ObservableCollection<DraftTeam> Teams { get; set; }

        public DraftTeam Team { get; set; }

        private void TeamSelect_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            TitleMessage.Text = IsServer ? "Set up Teams" : "Select Your Team";

            tStack.ItemsSource = Teams;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                var index = (int)button.Tag;
                var panel = VisualTreeHelper.GetParent(button) as WrapPanel;

                var textBox = panel.Children[0] as TextBox;
                if (textBox != null)
                {
                    var team = Teams.First(t => t.Index == index);
                    team.IsConnected = true;
                    Team = team;
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
