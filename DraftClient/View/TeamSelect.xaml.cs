

namespace DraftClient.View
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
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

        public ObservableCollection<DraftTeam> Teams { get; set; }

        public DraftTeam Team { get; set; }

        private void TeamSelect_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            tStack.ItemsSource = Teams;
        }

        private void TeamSelect_OnClosing(object sender, CancelEventArgs e)
        {
            if (Team == null)
            {
                e.Cancel = true;
            }
        }
    }
}
