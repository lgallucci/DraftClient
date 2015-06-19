namespace DraftClient.View
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;
    using DraftClient.ViewModel;

    /// <summary>
    /// Interaction logic for DraftController.xaml
    /// </summary>
    public partial class DraftTimerControl : UserControl
    {
        private Dictionary<int, string> SoundFiles = new Dictionary<int, string>
        {
        };

        private DispatcherTimer _timer;
        
        //TODO: Send timer info over wire
        //TODO: Play sounds
        public DraftTimerControl()
        {
            InitializeComponent();
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            _timer.Tick += (sender, args) =>
            {
                if (State == null) return;

                var pausedTicks = State.PickPauseTime > DateTime.MinValue ? (DateTime.UtcNow - State.PickPauseTime) : new TimeSpan(0);
                var timeLeft = State.PickEndTime + pausedTicks + State.PausedTime - DateTime.UtcNow;

                if (timeLeft.TotalSeconds < 10 && timeLeft.Seconds % 2 == 1)
                {
                    CountdownTextBlock.Foreground = (SolidColorBrush) (new BrushConverter().ConvertFrom("#DD0000"));
                }
                else
                {
                    CountdownTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("LimeGreen"));
                }

                if (timeLeft.TotalSeconds >= 1)
                {
                    CountdownTextBlock.Text = timeLeft.ToString(@"mm\:ss");
                }
                else
                {
                    if (timeLeft.TotalDays > 0)
                    CountdownTextBlock.Foreground = (SolidColorBrush) (new BrushConverter().ConvertFrom("#DD0000"));
                    CountdownTextBlock.Text = "00:00";
                }
            };
        }


        private void PlaySound(string uriPath)
        {
            Uri uri = new Uri(@"pack://application:,,,/Resources/movepoint.wav");
            var player = new MediaPlayer();
            player.Open(uri);
            player.Play();
        }

        public void PopulateState(DraftState state)
        {
            _timer.Stop();
            State = state;
            DataContext = State;
            _timer.Start();
        }

        private DraftState State { get; set; }


        private void ResetDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StartDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.Drafting = true;
            State.PickEndTime = DateTime.UtcNow + new TimeSpan(0, 0, 180);
            State.PickPauseTime = DateTime.MinValue;
            OnDraftStateChanged(State);
        }

        public void PauseDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.PickPauseTime = DateTime.UtcNow;
            OnDraftStateChanged(State);
        }

        public void ResumeDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.PausedTime = State.PausedTime + (DateTime.UtcNow - State.PickPauseTime);
            State.PickPauseTime = DateTime.MinValue;
            OnDraftStateChanged(State);
        }

        public void ResetDraftTimer()
        {
            State.PickPauseTime = DateTime.MinValue;
            State.PickEndTime = DateTime.UtcNow + new TimeSpan(0, 3, 0);
        }

        #region Events

        public delegate void DraftStateChangedHandler(DraftState state);

        public event DraftStateChangedHandler DraftStateChanged;

        public void OnDraftStateChanged(DraftState state)
        {
            DraftStateChangedHandler handler = DraftStateChanged;
            if (handler != null)
            {
                handler(state);
            }
        }

        #endregion
    }
}
