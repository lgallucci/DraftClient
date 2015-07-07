namespace DraftClient.View
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Media;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using DraftClient.ViewModel;

    /// <summary>
    /// Interaction logic for DraftController.xaml
    /// </summary>
    public partial class DraftTimerControl
    {
        private MediaPlayer _player;
        private readonly DispatcherTimer _timer;
        private DateTime _lastSoundPlayed;

        public DraftTimerControl()
        {
            InitializeComponent();
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            _player = new MediaPlayer();
            
            _timer.Tick += (sender, args) =>
            {
                if (State == null) return;

                var pausedTicks = State.PickPauseTime > DateTime.MinValue ? (DateTime.UtcNow - State.PickPauseTime) : new TimeSpan(0);
                var timeLeft = State.PickEndTime + pausedTicks + State.PausedTime - DateTime.UtcNow;

                if (timeLeft.TotalSeconds >= 30 && timeLeft.TotalSeconds < 31)
                {
                    PlaySound("glass_ping");
                }
                
                if (timeLeft.TotalSeconds < 10 && timeLeft.Seconds % 2 == 1)
                {
                    CountdownTextBlock.Foreground = (Brush)FindResource("AccentColorBrush3");
                }
                else
                {
                    if (timeLeft.TotalSeconds < 11 && timeLeft.TotalSeconds > 1)
                    {
                        PlaySound("countdown_beep");
                    }
                    CountdownTextBlock.Foreground = (Brush)FindResource("AccentColorBrush");
                }

                if (timeLeft.TotalSeconds >= 1)
                {
                    CountdownTextBlock.Text = timeLeft.ToString(@"mm\:ss");
                }
                else if (timeLeft.TotalSeconds < 1 && timeLeft.TotalSeconds >= 0)
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
            if (_lastSoundPlayed.AddSeconds(1) > DateTime.UtcNow)
            {
                return;
            }

            //Uri uri = new Uri(string.Format(@"pack://application:,,,/Resources/{0}.mp3", soundName));
            var uri = new Uri(string.Format(@"{0}\Resources\{1}.mp3", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), soundName));

            if (_player.Source != uri)
            {
                _player.Open(uri);
            } 
            _player.Position = new TimeSpan(0);
            _player.Play();

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
