﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DraftClient.View
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for FantasyTeam.xaml
    /// </summary>
    public partial class FantasyTeam
    {
        public int TeamNumber { get; set; }

        public FantasyTeam()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            CreateTextBlock(text);
        }

        private void TeamName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsServer && !IsMyTeam) return;
            CreateTextBox(RemoveElements());
        }

        public bool IsServer { get; set; }
        public bool IsMyTeam { get; set; }
        public bool IsConnected { get; set; }

        private void TeamNameEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TeamPanel.Children[0] as TextBox);
                if (textBox != null && textBox.Text != string.Empty)
                {
                    CreateTextBlock(RemoveElements());
                }
            }
        }

        private void TeamNameEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TeamPanel.Children[0] as TextBox);
            if (textBox != null && textBox.Text != string.Empty)
            {
                CreateTextBlock(RemoveElements());
            }
        }

        private void CreateTextBox(string text)
        {
            var textBox = new TextBox
            {
                Name = "TeamNameEdit",
                FontFamily = new System.Windows.Media.FontFamily("Franklin Gothic Medium"),
                FontSize = 16,
                Text = text
            };
            textBox.KeyUp += TeamNameEdit_KeyUp;
            textBox.LostFocus += TeamNameEdit_LostFocus;

            TeamPanel.Children.Add(textBox);

            textBox.Focus();
            textBox.SelectAll();
        }

        private void CreateTextBlock(string text)
        {
            var textBlock = new TextBlock
            {
                Name = "TeamName",
                FontFamily = new System.Windows.Media.FontFamily("Franklin Gothic Medium"),
                FontSize = 16,
                Text = text
            };
            textBlock.MouseUp += TeamName_MouseUp;
            TeamPanel.Children.Add(textBlock);
        }

        private string RemoveElements()
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

            return "";
        }


        public void SetConnected(bool isConnected)
        {
            ConnectedImage.Source = isConnected ?
                new BitmapImage(new Uri("pack://application:,,,/Resources/Connected.png")) 
                : new BitmapImage(new Uri("pack://application:,,,/Resources/Disconnected.png"));
        }
    }
}
