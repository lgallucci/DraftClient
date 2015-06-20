namespace DraftClient.ViewModel
{
    using System;

    public class DraftState : BindableBase
    {
        private readonly bool _isServer;
        private DateTime _pickEndTime;
        private DateTime _pickPauseTime;
        private bool _drafting;
        private int _draftSeconds;

        public DraftState(bool isServer, int numberOfSeconds)
        {
            PickEndTime = DateTime.UtcNow;
            PickPauseTime = DateTime.MinValue;
            _isServer = isServer;
            DraftSeconds = numberOfSeconds;
        }

        public DateTime PickEndTime
        {
            get { return _pickEndTime; }
            set
            {
                SetProperty(ref _pickEndTime, value);
                OnPropertyChanged("CanPause");
                OnPropertyChanged("CanResume");
                OnPropertyChanged("IsRunning");
            }
        }

        public DateTime PickPauseTime
        {
            get { return _pickPauseTime; }
            set
            {
                SetProperty(ref _pickPauseTime, value);
                OnPropertyChanged("CanPause");
                OnPropertyChanged("CanResume");
                OnPropertyChanged("IsRunning");
            }
        }

        public TimeSpan PausedTime { get; set; }

        public bool Drafting
        {
            get { return _drafting; }
            set { SetProperty(ref _drafting, value); }
        }

        public bool IsRunning
        {
            get { return _drafting && _isServer; }
        }

        public bool CanPause
        {
            get { return _isServer && _pickPauseTime == DateTime.MinValue; }
        }

        public bool CanResume
        {
            get { return _isServer && _pickPauseTime > DateTime.MinValue; }
        }

        public int DraftSeconds
        {
            get { return _draftSeconds; }
            set { SetProperty(ref _draftSeconds, value); }
        }
    }
}