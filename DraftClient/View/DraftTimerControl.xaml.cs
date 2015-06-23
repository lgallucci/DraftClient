namespace DraftClient.View
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using DraftClient.ViewModel;

    /// <summary>
    /// Interaction logic for DraftController.xaml
    /// </summary>
    public partial class DraftTimerControl
    {
        private Dictionary<int, string> SoundFiles = new Dictionary<int, string>
        {
        };

        private readonly DispatcherTimer _timer;
        private DateTime _lastSoundPlayed;

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
                    CountdownTextBlock.Foreground = (Brush)FindResource("AccentColorBrush3");
                }
                else
                {
                    CountdownTextBlock.Foreground = (Brush)FindResource("AccentColorBrush");
                }

                if (timeLeft.TotalSeconds >= 1)
                {
                    CountdownTextBlock.Text = timeLeft.ToString(@"mm\:ss");
                }
                else
                {
                    if (timeLeft.TotalDays > 0)
                        CountdownTextBlock.Foreground = (Brush)FindResource("AccentColorBrush3");
                    CountdownTextBlock.Text = "00:00";
                    PlaySound("buzzer");
                }
            };
        }
        
        private void PlaySound(string soundName)
        {
            if (_lastSoundPlayed.AddSeconds(1.1) > DateTime.UtcNow)
            {
                return;
            }

            Uri uri = new Uri(string.Format(@"pack://application:,,,/Resources/{0}.mp3", soundName));
            var player = new MediaPlayer();
            player.Open(uri);
            player.Play();

            _lastSoundPlayed = DateTime.UtcNow;
        }

        public void PopulateState(DraftState state)
        {
            _timer.Stop();
            State = state;
            DataContext = State;
            _timer.Start();
        }
        
        public void UpdateState(DateTime pickEndTime, DateTime pickPauseTime, bool drafting)
        {
            if (pickPauseTime == DateTime.MinValue && State.PickPauseTime > DateTime.MinValue)
            {
                State.PausedTime = State.PausedTime + (DateTime.UtcNow - State.PickPauseTime);
            }

            if (drafting && !State.Drafting)
            {
                State.PausedTime = new TimeSpan(0, 0, 0);
            }

            State.PickEndTime = pickEndTime;
            State.PickPauseTime = pickPauseTime;
            State.Drafting = drafting;
        }

        private DraftState State { get; set; }

        private void ResetDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.Drafting = false;
            State.PickPauseTime = DateTime.UtcNow;
            State.PickEndTime = DateTime.UtcNow + new TimeSpan(0, 0, State.DraftSeconds);
            OnDraftStateChanged(State);
        }

        private void StartDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.Drafting = true;
            State.PickEndTime = DateTime.UtcNow + new TimeSpan(0, 0, State.DraftSeconds);
            State.PickPauseTime = DateTime.MinValue;
            State.PausedTime = new TimeSpan(0,0,0);
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
