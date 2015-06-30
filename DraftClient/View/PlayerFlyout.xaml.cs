namespace DraftClient.View
{
    using System.Windows;
    using MahApps.Metro;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for PlayerFlyout.xaml
    /// </summary>
    public partial class PlayerFlyout : Flyout
    {
        public PlayerFlyout()
        {
            InitializeComponent();
        }

        public double LogoOpacity
        {
            get { return (ThemeManager.DetectAppStyle(Application.Current).Item1.Name == "BaseDark") ? .45 : .15; }
        }
    }
}
