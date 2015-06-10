namespace DraftClient.ViewModel
{
    public class DraftPick : BindableBase
    {
        private Player _draftedPlayer;
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_draftedPlayer != null && value.Trim().ToLower() != _draftedPlayer.Name.Trim().ToLower())
                {
                    _draftedPlayer.IsPicked = false;
                    SetProperty(ref _draftedPlayer, null);
                    SetProperty(ref _name, "");
                }
                else
                {
                    SetProperty(ref _name, value);
                }
            }
        }

        public bool CanEdit { get; set; }

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
                if (_draftedPlayer != null)
                {
                    _draftedPlayer.IsPicked = true;
                }
            }
        }
    }
}