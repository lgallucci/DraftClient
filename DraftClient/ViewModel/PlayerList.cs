namespace DraftClient.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
