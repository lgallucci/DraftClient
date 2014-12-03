namespace DraftClient.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DraftSettings : BindableBase
    {
        private string _leagueName = "Birches League";
        private int _numberOfTeams = 12;
        private int _quarterbacks = 1;
        private int _wideRecievers = 2;
        private int _runningBacks = 2;
        private int _flexWithTightEnd = 1;
        private int _flexWithoutTightEnd = 0;
        private int _tightEnds = 1;
        private int _kickers = 1;
        private int _defenses = 1;
        private int _benchPlayers = 4;
        private string _playerFile = @"C:\FantasyPlayerRankings.csv";

        public string LeagueName
        {
            get { return this._leagueName; }
            set
            {
                SetProperty(ref this._leagueName, value);
            }
        }
        public int NumberOfTeams
        {
            get { return this._numberOfTeams; }
            set
            {
                SetProperty(ref this._numberOfTeams, value);
            }
        }
        public int Quarterbacks
        {
            get { return this._quarterbacks; }
            set
            {
                SetProperty(ref this._quarterbacks, value);
            }
        }
        public int WideRecievers
        {
            get { return this._wideRecievers; }
            set
            {
                SetProperty(ref this._wideRecievers, value);
            }
        }
        public int RunningBacks
        {
            get { return this._runningBacks; }
            set
            {
                SetProperty(ref this._runningBacks, value);
            }
        }
        public int FlexWithTightEnd
        {
            get { return this._flexWithTightEnd; }
            set
            {
                SetProperty(ref this._flexWithTightEnd, value);
            }
        }
        public int FlexWithoutTightEnd
        {
            get { return this._flexWithoutTightEnd; }
            set
            {
                SetProperty(ref this._flexWithoutTightEnd, value);
            }
        }
        public int TightEnds
        {
            get { return this._tightEnds; }
            set
            {
                SetProperty(ref this._tightEnds, value);
            }
        }
        public int Kickers
        {
            get { return this._kickers; }
            set
            {
                SetProperty(ref this._kickers, value);
            }
        }
        public int Defenses
        {
            get { return this._defenses; }
            set
            {
                SetProperty(ref this._defenses, value);
            }
        }
        public int BenchPlayers
        {
            get { return this._benchPlayers; }
            set
            {
                SetProperty(ref this._benchPlayers, value);
            }
        }
        public string PlayerFile
        {
            get { return this._playerFile; }
            set
            {
                SetProperty(ref this._playerFile, value);
            }
        }
    }
}
