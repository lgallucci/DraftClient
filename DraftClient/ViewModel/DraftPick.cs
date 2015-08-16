namespace DraftClient.ViewModel
{
    public class DraftPick : BindableBase
    {
        private Player _draftedPlayer;
        private bool _canEdit;
        private bool _isLoading;

        public DraftPick()
        {
        }

        public DraftPick(Player player, bool canEdit, bool isLoading)
        {
            _draftedPlayer = player;
            _canEdit = canEdit;
            _isLoading = isLoading;
            
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set { SetProperty(ref _canEdit, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        public Player DraftedPlayer
        {
            get { return _draftedPlayer; }
            set
            {
                if (_draftedPlayer != null)
                {
                    _draftedPlayer.IsPicked = false;
                }
                SetProperty(ref _draftedPlayer, value);
                OnMakePick(_draftedPlayer == null ? 0 : _draftedPlayer.Rank);
                if (_draftedPlayer != null)
                {
                    _draftedPlayer.IsPicked = true;
                }
            }
        }

        #region Events

        public delegate void MakePickHandler(int adp);

        public event MakePickHandler MakePick;

        public void OnMakePick(int adp)
        {
            MakePickHandler handler = MakePick;
            if (handler != null)
            {
                handler(adp);
            }
        }

        #endregion

    }
}