namespace DraftClient.View
{
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
