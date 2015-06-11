namespace DraftClient.ViewModel
{
    using System;

    public class DraftState : BindableBase
    {
        private bool _isServer = false;
        private DateTime _pickEndTime;
        private DateTime _pickPauseTime;
        private bool _drafting;

        public DraftState(bool isServer)
        {
            PickEndTime = (DateTime.UtcNow + new TimeSpan(0, 0, 30));
            PickPauseTime = DateTime.MinValue;
            _isServer = isServer;
        }
        public DateTime PickEndTime
        {
            get { return _pickEndTime; }
            set
            {
                SetProperty(ref _pickEndTime, value);
                OnPropertyChanged("CanPause");
                OnPropertyChanged("CanResume");
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
            }
        }

        public TimeSpan PausedTime { get; set; }

        public bool Drafting
        {
            get { return _drafting; }
            set { SetProperty(ref _drafting, value); }
        }

        public bool CanPause
        {
            get { return _isServer && _pickPauseTime == DateTime.MinValue; }
        }

        public bool CanResume
        {
            get { return _isServer && _pickPauseTime > DateTime.MinValue; }
        }
    }
}
