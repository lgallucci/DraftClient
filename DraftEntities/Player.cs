﻿namespace DraftEntities
{
    using System;

    [Serializable]
    public class Player
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public PlayerPosition Position { get; set; }
        public string Team { get; set; }
        public int ByeWeek { get; set; }
        public decimal ProjectedPoints { get; set; }
        public int Age { get; set; }
        public int SuspendedGames { get; set; }
        public int PlayerId { get; set; }
    }
}