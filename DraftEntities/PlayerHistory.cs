namespace DraftEntities
{
    using System;

    [Serializable]
    public class PlayerHistory
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public int Age { get; set; }
        public int GamesPlayed { get; set; }
        public int PassingYards { get; set; }
        public int PassingTouchdowns { get; set; }
        public int PassingInterceptions { get; set; }
        public int RushingAttempts { get; set; }
        public int RushingYards { get; set; }
        public int RushingTouchdowns { get; set; }
        public int Receptions { get; set; }
        public int ReceivingYards { get; set; }
        public int ReceivingTouchdowns { get; set; }
        public decimal FieldGoalPercentage { get; set; }
        public int ZeroToThirtyNine { get; set; }
        public int FourtyToFourtyNine { get; set; }
        public int FiftyPlus { get; set; }
        public int ExtraPoints { get; set; }
        public int DefensiveSacks { get; set; }
        public int DefensiveTouchdowns { get; set; }
        public int DefensiveInterceptions { get; set; }
        public int DefensiveFumblesRecovered { get; set; }
        public int DefensivePointsAgainst { get; set; }
        public decimal FantasyPoints { get; set; }
        public int PositionRank { get; set; }
        public int OverallRank { get; set; }
    }
}
