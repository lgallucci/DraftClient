namespace DraftClient.ViewModel
{
    using System.Collections.ObjectModel;

    public class DraftSettings : ValidatableBase
    {
        private string _leagueName = "";
        private int _numberOfTeams;
        private int _quarterbacks = 1;
        private int _wideRecievers = 2;
        private int _runningBacks = 2;
        private int _flexWithTightEnd = 1;
        private int _tightEnds = 1;
        private int _kickers = 1;
        private int _defenses = 1;
        private int _benchPlayers = 4;
        private string _playerFile = @"C:\FantasyPlayerRankings.csv";
        private ObservableCollection<DraftServer> _servers;
        private ObservableCollection<DraftTeam> _draftTeams;

        public string LeagueName
        {
            get { return _leagueName; }
            set
            {
                SetProperty(ref _leagueName, value);
            }
        }
        public int NumberOfTeams
        {
            get { return _numberOfTeams; }
            set
            {
                SetProperty(ref _numberOfTeams, value);
                DraftTeams.Clear();
                for(int i = 0; i < _numberOfTeams; i++)
                {
                    DraftTeams.Add(new DraftTeam
                    {
                        Index = i,
                        IsConnected = false,
                        Name = string.Format("Team{0}", i+1)
                    });
                }
            }
        }
        public int Quarterbacks
        {
            get { return _quarterbacks; }
            set
            {
                SetProperty(ref _quarterbacks, value);
            }
        }
        public int WideRecievers
        {
            get { return _wideRecievers; }
            set
            {
                SetProperty(ref _wideRecievers, value);
            }
        }
        public int RunningBacks
        {
            get { return _runningBacks; }
            set
            {
                SetProperty(ref _runningBacks, value);
            }
        }
        public int FlexWithTightEnd
        {
            get { return _flexWithTightEnd; }
            set
            {
                SetProperty(ref _flexWithTightEnd, value);
            }
        }
        public int TightEnds
        {
            get { return _tightEnds; }
            set
            {
                SetProperty(ref _tightEnds, value);
            }
        }
        public int Kickers
        {
            get { return _kickers; }
            set
            {
                SetProperty(ref _kickers, value);
            }
        }
        public int Defenses
        {
            get { return _defenses; }
            set
            {
                SetProperty(ref _defenses, value);
            }
        }
        public int BenchPlayers
        {
            get { return _benchPlayers; }
            set
            {
                SetProperty(ref _benchPlayers, value);
            }
        }
        public string PlayerFile
        {
            get { return _playerFile; }
            set
            {
                SetProperty(ref _playerFile, value);
            }
        }
        public ObservableCollection<DraftServer> Servers
        {
            get { return _servers ?? (_servers = new ObservableCollection<DraftServer>()); }
            set
            {
                SetProperty(ref _servers, value);
            }
        }
        public ObservableCollection<DraftTeam> DraftTeams
        {
            get { return _draftTeams ?? (_draftTeams = new ObservableCollection<DraftTeam>()); }
            set
            {
                SetProperty(ref _draftTeams, value);
            }
        }
        public int TotalRounds
        {
            get
            {
                return Quarterbacks + WideRecievers + RunningBacks + FlexWithTightEnd + TightEnds + Kickers + Defenses + BenchPlayers;
            }
        }
        public override bool Validate()
        {
            return _leagueName.Length > 0
                   && _numberOfTeams > 0 
                   && _numberOfTeams < 15;
        }
    }
}
