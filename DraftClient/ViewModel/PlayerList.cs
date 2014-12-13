namespace DraftClient.ViewModel
{
    using System.Collections.ObjectModel;

    public class PlayerList : BindableBase
    {
        public PlayerList()
        {
            _players = new ObservableCollection<PlayerPresentation>();
        }

        private ObservableCollection<PlayerPresentation> _players;
        public ObservableCollection<PlayerPresentation> Players
        {
            get
            {
                return this._players;
            }
            set
            {
                SetProperty(ref this._players, value);
            }
        }
    }
}
