namespace DraftClient.View
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using ClientServer;

    /// <summary>
    ///     Interaction logic for FantasyTeam.xaml
    /// </summary>
    public partial class FantasyTeam
    {
        public FantasyTeam()
        {
            InitializeComponent();
        }

        public int TeamNumber { get; set; }

        public bool IsServer { get; set; }
        public bool IsMyTeam { get; set; }
        public bool IsConnected { get; set; }

        public void SetText(string text)
        {
            RemoveElements();
            CreateTextBlock(text);
        }

        private void TeamName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsServer && !IsMyTeam)
            {
                return;
            }
            CreateTextBox(RemoveElements());
        }

        private void TeamNameEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TeamPanel.Children[0] as TextBox);
                if (textBox != null && textBox.Text != string.Empty)
                {
                    CreateTextBlock(RemoveElements(), true);
                }
            }
        }

        private void TeamNameEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TeamPanel.Children[0] as TextBox);
            if (textBox != null && textBox.Text != string.Empty)
            {
                CreateTextBlock(RemoveElements(), true);
            }
        }

        private void CreateTextBox(string text)
        {
            var textBox = new TextBox
            {
                Name = "TeamNameEdit",
                FontFamily = new FontFamily("Franklin Gothic Medium"),
                FontSize = 16,
                Text = text
            };
            textBox.KeyUp += TeamNameEdit_KeyUp;
            textBox.LostFocus += TeamNameEdit_LostFocus;

            TeamPanel.Children.Add(textBox);

            textBox.Focus();
            textBox.SelectAll();
        }

        private void CreateTextBlock(string text, bool triggerEvent = false)
        {
            if (triggerEvent)
            {
                OnTeamChanged(TeamNumber, text);
            }
            var textBlock = new TextBlock
            {
                Name = "TeamName",
                FontFamily = new FontFamily("Franklin Gothic Medium"),
                FontSize = 16,
                Text = text
            };
            textBlock.MouseUp += TeamName_MouseUp;
            TeamPanel.Children.Add(textBlock);
        }

        private string RemoveElements()
        {
            if (TeamPanel.Children.Count > 0)
            {
                var textBox = TeamPanel.Children[0] as TextBox;
                if (textBox != null)
                {
                    textBox.KeyUp -= TeamNameEdit_KeyUp;
                    textBox.LostFocus -= TeamNameEdit_LostFocus;
                    TeamPanel.Children.Clear();
                    return textBox.Text;
                }

                var textBlock = TeamPanel.Children[0] as TextBlock;
                if (textBlock != null)
                {
                    textBlock.MouseUp -= TeamName_MouseUp;
                    TeamPanel.Children.Clear();
                    return textBlock.Text;
                }
            }

            return "";
        }


        public void SetConnected(bool isConnected, bool isJoined)
        {
            if (!isJoined && !isConnected)
            {
                ConnectedImage.Visibility = Visibility.Collapsed;
                JoinButton.Visibility = Visibility.Visible;
            }
            else
            {
                ConnectedImage.Visibility = Visibility.Visible;
                ConnectedImage.Source = isConnected
                    ? new BitmapImage(new Uri("pack://application:,,,/Resources/Connected.png"))
                    : new BitmapImage(new Uri("pack://application:,,,/Resources/Disconnected.png"));
                JoinButton.Visibility = Visibility.Collapsed;
            }
        }

        private void JoinButton_OnClick(object sender, RoutedEventArgs e)
        {
            OnTeamJoined(TeamNumber);
        }

        #region events

        public delegate void TeamChangedHandler(int teamNumber, string name);
        public event TeamChangedHandler TeamChanged;

        public void OnTeamChanged(int teamNumber, string name)
        {
            TeamChangedHandler handler = TeamChanged;
            if (handler != null)
            {
                handler(teamNumber, name);
            }
        }

        public delegate void TeamJoinedHandler(int teamNumber);
        public event TeamJoinedHandler TeamJoined;

        public void OnTeamJoined(int teamNumber)
        {
            TeamJoinedHandler handler = TeamJoined;
            if (handler != null)
            {
                handler(teamNumber);
            }
        }

        #endregion
    }
}