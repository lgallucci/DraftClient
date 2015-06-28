namespace DraftClient.ViewModel
{
    public class PlayerHistory : BindableBase
    {
        private int _playerId;
        private string _name;
        private int _year;
        private string _team;
        private string _position;
        private int _age;
        private int _gamesPlayed;
        private int _passingYards;
        private int _passingTouchdowns;
        private int _passingInterceptions;
        private int _rushingAttempts;
        private int _rushingYards;
        private int _rushingTouchdowns;
        private int _receptions;
        private int _receivingYards;
        private int _receivingTouchdowns;
        private decimal _fieldGoalPercentage;
        private decimal _fantasyPoints;
        private int _positionRank;
        private int _overallRank;

        public int PlayerId
        {
            get { return _playerId; }
            set { SetProperty(ref _playerId, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public int Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }

        public string Team
        {
            get { return _team; }
            set { SetProperty(ref _team, value); }
        }

        public string Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public int Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        public int GamesPlayed
        {
            get { return _gamesPlayed; }
            set { SetProperty(ref _gamesPlayed, value); }
        }

        public int PassingYards
        {
            get { return _passingYards; }
            set { SetProperty(ref _passingYards, value); }
        }

        public int PassingTouchdowns
        {
            get { return _passingTouchdowns; }
            set { SetProperty(ref _passingTouchdowns, value); }
        }

        public int PassingInterceptions
        {
            get { return _passingInterceptions; }
            set { SetProperty(ref _passingInterceptions, value); }
        }

        public int RushingAttempts
        {
            get { return _rushingAttempts; }
            set { SetProperty(ref _rushingAttempts, value); }
        }

        public int RushingYards
        {
            get { return _rushingYards; }
            set { SetProperty(ref _rushingYards, value); }
        }

        public int RushingTouchdowns
        {
            get { return _rushingTouchdowns; }
            set { SetProperty(ref _rushingTouchdowns, value); }
        }

        public int Receptions
        {
            get { return _receptions; }
            set { SetProperty(ref _receptions, value); }
        }

        public int ReceivingYards
        {
            get { return _receivingYards; }
            set { SetProperty(ref _receivingYards, value); }
        }

        public int ReceivingTouchdowns
        {
            get { return _receivingTouchdowns; }
            set { SetProperty(ref _receivingTouchdowns, value); }
        }

        public decimal FieldGoalPercentage
        {
            get { return _fieldGoalPercentage; }
            set { SetProperty(ref _fieldGoalPercentage, value); }
        }

        public decimal FantasyPoints
        {
            get { return _fantasyPoints; }
            set { SetProperty(ref _fantasyPoints, value); }
        }

        public int PositionRank
        {
            get { return _positionRank; }
            set { SetProperty(ref _positionRank, value); }
        }

        public int OverallRank
        {
            get { return _overallRank; }
            set { SetProperty(ref _overallRank, value); }
        }
    }
}