namespace DraftClient.View
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;

    /// <summary>
    /// Interaction logic for SpinnyWindow.xaml
    /// </summary>
    public partial class SpinnyWindow
    {

        public SpinnyWindow()
        {
            InitializeComponent();
        }

        public void ShowWithText(string loadingMessage)
        {
            LoadingMessage.Text = loadingMessage;
            Show();
        }
    }
}
