namespace DraftClient.ViewModel
{
    public class DraftPick : BindableBase
    {
        private Player _draftedPlayer;
        private string _name;
        private bool _canEdit;
        private bool _isLoading;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_draftedPlayer != null && value.Trim().ToLower() != _draftedPlayer.Name.Trim().ToLower())
                {
                    SetProperty(ref _draftedPlayer, null);
                    SetProperty(ref _name, "");
                }
                else
                {
                    SetProperty(ref _name, value);
                }
            }
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