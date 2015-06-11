namespace DraftClient.ViewModel
{
    using System.Collections.ObjectModel;

    public class PlayerList : BindableBase
    {
        private ObservableCollection<Player> _players;

        public PlayerList()
        {
            _players = new ObservableCollection<Player>();
        }

        public ObservableCollection<Player> Players
        {
            get { return _players; }
            set { SetProperty(ref _players, value); }
        }
    }
}