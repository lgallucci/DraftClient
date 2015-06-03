namespace DraftClient.ViewModel
{
    using System.Collections.ObjectModel;

    public class PlayerList : BindableBase
    {
        public PlayerList()
        {
            _players = new ObservableCollection<Player>();
        }

        private ObservableCollection<Player> _players;
        public ObservableCollection<Player> Players
        {
            get
            {
                return _players;
            }
            set
            {
                SetProperty(ref _players, value);
            }
        }
    }
}
