namespace DraftClient.ViewModel
{
    public class DraftPick : BindableBase
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_draftedPlayer != null && value.Trim().ToLower() != _draftedPlayer.Name.Trim().ToLower())
                {
                    _draftedPlayer.IsPicked = false;
                    SetProperty(ref this._draftedPlayer, null);
                    SetProperty(ref this._name, "");
                }
                else
                {
                    SetProperty(ref this._name, value);
                }
            }
        }

        public bool CanEdit
        {
            get; set;
        }

        private PlayerPresentation _draftedPlayer;
        public PlayerPresentation DraftedPlayer
        {
            get
            {
                return _draftedPlayer;
            }
            set
            {
                if (_draftedPlayer != null)
                {
                    _draftedPlayer.IsPicked = false;
                }
                SetProperty(ref this._draftedPlayer, value);
                if (_draftedPlayer != null)
                {
                    _draftedPlayer.IsPicked = true;
                }
            }
        }
    }
}
