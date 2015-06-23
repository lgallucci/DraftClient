namespace DraftClient.ViewModel
{
    using System.Collections.ObjectModel;

    public class PlayerList : BindableBase
    {
        private ObservableCollection<Player> _players;
        private ObservableCollection<TeamSchedule> _schedules;
        private ObservableCollection<PlayerHistory> _history;

        public PlayerList()
        {
            _players = new ObservableCollection<Player>();
            _schedules = new ObservableCollection<TeamSchedule>();
            _history = new ObservableCollection<PlayerHistory>();
        }

        public ObservableCollection<Player> Players
        {
            get { return _players; }
            set { SetProperty(ref _players, value); }
        }

        public ObservableCollection<TeamSchedule> Schedules
        {
            get { return _schedules; }
            set { SetProperty(ref _schedules, value); }
        }

        public ObservableCollection<PlayerHistory> Histories
        {
            get { return _history; }
            set { SetProperty(ref _history, value); }
        }

    }
}