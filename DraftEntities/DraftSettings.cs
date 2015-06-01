namespace DraftEntities
{
    using System;

    [Serializable]
    public class DraftSettings
    {
        public string LeagueName { get; set; }
        public int NumberOfTeams { get; set; }
        public int Quarterbacks { get; set; }
        public int WideRecievers { get; set; }
        public int RunningBacks { get; set; }
        public int FlexWithTightEnd { get; set; }
        public int FlexWithoutTightEnd { get; set; }
        public int TightEnds { get; set; }
        public int Kickers { get; set; }
        public int Defenses { get; set; }
        public int BenchPlayers { get; set; }
    }
}
