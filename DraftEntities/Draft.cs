namespace DraftEntities
{
    using System;

    [Serializable]
    public class Draft
    {
        public long PickEndTime { get; set; }
        public long PickPauseTime { get; set; }
        public int Round { get; set; }
        public int MaxRound { get; set; }
        public int Team { get; set; }
        public int MaxTeam { get; set; }
        public bool Drafting { get; set; }
        public int[,] Picks { get; set; }
    }
}
