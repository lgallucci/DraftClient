namespace DraftClient.View
{
    using System;
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
        private DispatcherTimer _timer;

        public DraftTimerControl()
        {
            InitializeComponent();
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            _timer.Tick += (sender, args) =>
            {
                var timeLeft = new TimeSpan(DateTime.UtcNow.Ticks - State.PickEndTime);

                if (timeLeft.TotalSeconds < 10 && timeLeft.Seconds % 2 == 1)
                {
                    CountdownTextBlock.Foreground = (SolidColorBrush) (new BrushConverter().ConvertFrom("#DD0000"));
                }
                else
                {
                    CountdownTextBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("LimeGreen"));
                }

                if (timeLeft.TotalSeconds > 0)
                {
                    CountdownTextBlock.Text = timeLeft.ToString(@"mm\:ss");
                }
                else
                {
                    CountdownTextBlock.Foreground = (SolidColorBrush) (new BrushConverter().ConvertFrom("#DD0000"));
                    CountdownTextBlock.Text = "00:00";
                }
            };
            _timer.Start();
        }

        public void PopulateState(DraftState state)
        {
            State = state;
            DataContext = State;
        }

        private DraftState State { get; set; }

        private void StartDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.Drafting = true;
            State.PickEndTime = (DateTime.UtcNow + new TimeSpan(0, 3, 0)).Ticks;
            State.PickPauseTime = -1;
            OnDraftStateChanged(State);
        }

        public void PauseDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.PickPauseTime = DateTime.UtcNow.Ticks;
            OnDraftStateChanged(State);
        }

        public void ResumeDraftButton_OnClick(object sender, RoutedEventArgs e)
        {
            State.PickPauseTime = -1;
            State.PickEndTime = (State.PickEndTime - State.PickPauseTime) + DateTime.UtcNow.Ticks;
            OnDraftStateChanged(State);
        }

        public void ResetDraftTimer()
        {
            State.PickPauseTime = -1;
            State.PickEndTime = (DateTime.UtcNow + new TimeSpan(0, 3, 0)).Ticks;
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
