namespace DraftClient.ViewModel
{
    using System;

    public class DraftState : BindableBase
    {
        private bool _isServer = false;
        private long _pickEndTime;
        private long _pickPauseTime;
        private bool _drafting;

        public DraftState(bool isServer)
        {
            PickEndTime = (DateTime.Now + new TimeSpan(0, 0, 180)).Ticks;
            PickPauseTime = -1;
            _isServer = isServer;
        }
        public long PickEndTime
        {
            get { return _pickEndTime; }
            set
            {
                SetProperty(ref _pickEndTime, value);
                OnPropertyChanged("CanPause");
                OnPropertyChanged("CanResume");
            }
        }
        public long PickPauseTime
        {
            get { return _pickPauseTime; }
            set
            {
                SetProperty(ref _pickPauseTime, value);
                OnPropertyChanged("CanPause");
                OnPropertyChanged("CanResume");
            }
        }
        public bool Drafting
        {
            get { return _drafting; }
            set
            {
                SetProperty(ref _drafting, value);
            }
        }

        public bool CanPause
        {
            get { return _isServer && _pickPauseTime <= 0; }
        }

        public bool CanResume
        {
            get { return _isServer && _pickPauseTime > 0; }
        }
    }
}
