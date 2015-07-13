namespace DraftClient.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using DraftEntities;

    public class Player : BindableBase
    {
        private int _Rank;
        private int _byeWeek;
        private bool _isPicked;
        private string _name;
        private PlayerPosition _position;
        private decimal _projectedPoints;
        private string _team;
        private int _age;
        private int _playerId;
        private Rect _logoRectangle = new Rect(0, 0, 350, 350);
        private List<PlayerHistory> _histories = new List<PlayerHistory>();
        private TeamSchedule _schedule;
        private int _suspendedGames;

        public int Rank
        {
            get { return _Rank; }
            set { SetProperty(ref _Rank, value); }
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

        public int Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
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

        public int SuspendedGames
        {
            get { return _suspendedGames; }
            set { SetProperty(ref _suspendedGames, value); }
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
                    return new Rect(0, 0, 350, 350);
                case "ATL":
                    return new Rect(350, 0, 350, 350);
                case "BAL":
                    return new Rect(700, 0, 350, 350);
                case "BUF":
                    return new Rect(1050, 0, 350, 350);
                case "CAR":
                    return new Rect(1400, 0, 350, 350);
                case "CHI":
                    return new Rect(1750, 0, 350, 350);
                case "CIN":
                    return new Rect(0, 350, 350, 350);
                case "CLE":
                    return new Rect(350, 350, 350, 350);
                case "DAL":
                    return new Rect(700, 350, 350, 350);
                case "DEN":
                    return new Rect(1050, 350, 350, 350);
                case "DET":
                    return new Rect(1400, 350, 350, 350);
                case "GB":
                    return new Rect(1750, 350, 350, 350);
                case "HOU":
                    return new Rect(0, 700, 350, 350);
                case "IND":
                    return new Rect(350, 700, 350, 350);
                case "JAC":
                    return new Rect(700, 700, 350, 350);
                case "KC":
                    return new Rect(1050, 700, 350, 350);
                case "MIA":
                    return new Rect(1400, 700, 350, 350);
                case "MIN":
                    return new Rect(1750, 700, 350, 350);
                case "NE":
                    return new Rect(0, 1050, 350, 350);
                case "NO":
                    return new Rect(350, 1050, 350, 350);
                case "NYG":
                    return new Rect(700, 1050, 350, 350);
                case "NYJ":
                    return new Rect(1050, 1050, 350, 350);
                case "OAK":
                    return new Rect(1400, 1050, 350, 350);
                case "PHI":
                    return new Rect(1750, 1050, 350, 350);
                case "PIT":
                    return new Rect(0, 1400, 350, 350);
                case "SD":
                    return new Rect(350, 1400, 350, 350);
                case "SF":
                    return new Rect(700, 1400, 350, 350);
                case "SEA":
                    return new Rect(1050, 1400, 350, 350);
                case "STL":
                    return new Rect(1400, 1400, 350, 350);
                case "TB":
                    return new Rect(1750, 1400, 350, 350);
                case "TEN":
                    return new Rect(0, 1750, 350, 350);
                case "WAS":
                    return new Rect(350, 1750, 350, 350);
            }

            return new Rect(0, 0, 350, 350);
        }

        protected override void AfterPropertyChanged(string propertyName)
        {
            if (propertyName == "Histories")
            {
                OnPropertyChanged("CanSeePassing");
                OnPropertyChanged("CanSeeRushing");
                OnPropertyChanged("CanSeeReceiving");
                OnPropertyChanged("CanSeeKicking");
                OnPropertyChanged("CanSeeDefense");
                OnPropertyChanged("IsRookie");
            }

            if (propertyName == "SuspendedGames")
            {
                OnPropertyChanged("IsSuspended");
            }
        }

        public bool CanSeePassing { get { return _position == PlayerPosition.QB && !IsRookie; } }

        public bool CanSeeRushing { get { return (_position == PlayerPosition.RB || _position == PlayerPosition.QB) && !IsRookie; } }

        public bool CanSeeReceiving { get { return (_position == PlayerPosition.RB || _position == PlayerPosition.WR || _position == PlayerPosition.TE) && !IsRookie; } }

        public bool CanSeeKicking { get { return _position == PlayerPosition.K && !IsRookie; } }

        public bool CanSeeDefense { get { return _position == PlayerPosition.DEF && !IsRookie; } }

        public bool IsRookie { get { return _histories.Count == 0; } }

        public bool IsSuspended { get { return _suspendedGames > 0; } }
    }
}