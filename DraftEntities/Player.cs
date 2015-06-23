namespace DraftEntities
{
    using System;

    [Serializable]
    public class Player
    {
        public int AverageDraftPosition { get; set; }
        public string Name { get; set; }
        public PlayerPosition Position { get; set; }
        public string Team { get; set; }
        public int ByeWeek { get; set; }
        public decimal ProjectedPoints { get; set; }
        public int PlayerId { get; set; }
    }
}