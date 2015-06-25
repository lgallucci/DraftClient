namespace DraftClient.ViewModel
{
    using System.Windows;
    using DraftEntities;

    public class Player : BindableBase
    {
        private int _averageDraftPosition;
        private int _byeWeek;
        private bool _isPicked;
        private string _name;
        private PlayerPosition _position;
        private decimal _projectedPoints;
        private string _team;
        private int _playerId;
        private Int32Rect _logoRectangle;

        public int AverageDraftPosition
        {
            get { return _averageDraftPosition; }
            set { SetProperty(ref _averageDraftPosition, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public PlayerPosition Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public string Team
        {
            get { return _team; }
            set
            {
                SetProperty(ref _team, value);
                LogoRectangle = GetLogoRectangle(_team);
            }
        }

        public int ByeWeek
        {
            get { return _byeWeek; }
            set { SetProperty(ref _byeWeek, value); }
        }

        public decimal ProjectedPoints
        {
            get { return _projectedPoints; }
            set { SetProperty(ref _projectedPoints, value); }
        }

        public int PlayerId
        {
            get { return _playerId; }
            set { SetProperty(ref _playerId, value); }
        }

        public bool IsPicked
        {
            get { return _isPicked; }
            set { SetProperty(ref _isPicked, value); }
        }

        public Int32Rect LogoRectangle
        {
            get { return _logoRectangle; }
            set { SetProperty(ref _logoRectangle, value); }
        }

        private Int32Rect GetLogoRectangle(string team)
        {
            switch (team)
            {
                case "CHI":
                    return new Int32Rect(2000, 0, 400, 400);
            }

            return new Int32Rect(0, 0, 400, 400);
        }
    }
}