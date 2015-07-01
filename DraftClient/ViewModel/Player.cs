namespace DraftClient.ViewModel
{
    using System;
    using System.Collections.Generic;
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
        private Rect _logoRectangle = new Rect(0, 0, 400, 400);
        private List<PlayerHistory> _histories = new List<PlayerHistory>();
        private TeamSchedule _schedule;

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
            get { return Decimal.Round(_projectedPoints, 1); }
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

        public Rect LogoRectangle
        {
            get { return _logoRectangle; }
            set { SetProperty(ref _logoRectangle, value); }
        }

        public List<PlayerHistory> Histories
        {
            get { return _histories; }
            set { SetProperty(ref _histories, value); }
        }

        public TeamSchedule Schedule
        {
            get { return _schedule; }
            set { SetProperty(ref _schedule, value); }
        } 

        private Rect GetLogoRectangle(string team)
        {
            switch (team)
            {
                case "ARI":
                    return new Rect(0, 0, 400, 400);
                case "ATL":
                    return new Rect(400, 0, 400, 400);
                case "BAL":
                    return new Rect(800, 0, 400, 400);
                case "BUF":
                    return new Rect(1200, 0, 400, 400);
                case "CAR":
                    return new Rect(1600, 0, 400, 400);
                case "CHI":
                    return new Rect(2000, 0, 400, 400);
                case "CIN":
                    return new Rect(0, 400, 400, 400);
                case "CLE":
                    return new Rect(400, 400, 400, 400);
                case "DAL":
                    return new Rect(800, 400, 400, 400);
                case "DEN":
                    return new Rect(1200, 400, 400, 400);
                case "DET":
                    return new Rect(1600, 400, 400, 400);
                case "GB":
                    return new Rect(2000, 400, 400, 400);
                case "HOU":
                    return new Rect(0, 800, 400, 400);
                case "IND":
                    return new Rect(400, 800, 400, 400);
                case "JAC":
                    return new Rect(800, 800, 400, 400);
                case "KC":
                    return new Rect(1200, 800, 400, 400);
                case "MIA":
                    return new Rect(1600, 800, 400, 400);
                case "MIN":
                    return new Rect(2000, 800, 400, 400);
                case "NE":
                    return new Rect(0, 1200, 400, 400);
                case "NO":
                    return new Rect(400, 1200, 400, 400);
                case "NYG":
                    return new Rect(800, 1200, 400, 400);
                case "NYJ":
                    return new Rect(1200, 1200, 400, 400);
                case "OAK":
                    return new Rect(1600, 1200, 400, 400);
                case "PHI":
                    return new Rect(2000, 1200, 400, 400);
                case "PIT":
                    return new Rect(0, 1600, 400, 400);
                case "SD":
                    return new Rect(400, 1600, 400, 400);
                case "SF":
                    return new Rect(800, 1600, 400, 400);
                case "SEA":
                    return new Rect(1200, 1600, 400, 400);
                case "STL":
                    return new Rect(1600, 1600, 400, 400);
                case "TB":
                    return new Rect(2000, 1600, 400, 400);
                case "TEN":
                    return new Rect(0, 2000, 400, 400);
                case "WAS":
                    return new Rect(400, 2000, 400, 400);
            }

            return new Rect(0, 0, 400, 400);
        }

        protected override void AfterPropertyChanged(string propertyName)
        {
            if (propertyName == "Position")
            {
                OnPropertyChanged("CanSeePassing");
                OnPropertyChanged("CanSeeRushing");
                OnPropertyChanged("CanSeeReceiving");
                OnPropertyChanged("CanSeeKicking");
                OnPropertyChanged("CanSeeDefense");
            }
        }

        public bool CanSeePassing { get { return _position == PlayerPosition.QB; } }

        public bool CanSeeRushing { get { return _position == PlayerPosition.RB || _position == PlayerPosition.QB; } }

        public bool CanSeeReceiving { get { return _position == PlayerPosition.RB || _position == PlayerPosition.WR || _position == PlayerPosition.TE; } }

        public bool CanSeeKicking { get { return _position == PlayerPosition.K; } }

        public bool CanSeeDefense { get { return _position == PlayerPosition.DEF; } }
    }
}