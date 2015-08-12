namespace DataFileCreator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FileHandler;
    using DraftEntities;
    using Omu.ValueInjecter;

    public class PlayerHistoryTemp
    {
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

    public class AgesTemp
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PlayerTemp
    {
        public int playerId { get; set; }
        public string player { get; set; }
        public string position { get; set; }
        public string team { get; set; }
        public string vor { get; set; }
        public decimal points { get; set; }
        public string actualPoints { get; set; }
        public string overallECR { get; set; }
        public int overallRank { get; set; }
        public string positionRank { get; set; }
        public string dropoff { get; set; }
        public string adp { get; set; }
        public string adpdiff { get; set; }
        public string auctionValue { get; set; }
        public string upper { get; set; }
        public string lower { get; set; }
        public string risk { get; set; }
        public string sleeper { get; set; }

    }

    public class DataFileCreator
    {
        public static void Main()
        {
            var players = DraftFileHandler.ReadCsvFile<Player>("FantasyPlayers.csv");

            //UpdateByeWeeks(players);
            //UpdateHistories(players);
            //UpdateAges(players);
            UpdateRanks(players);
        }

        private static void UpdateRanks(List<Player> players)
        {
            var playerRanks = DraftFileHandler.ReadCsvFile<PlayerTemp>("player_ranks.csv");

            foreach (var player in players)
            {
                var newRank = playerRanks.FirstOrDefault(r => r.playerId == player.PlayerId);
                if (newRank != null)
                {
                    player.Rank = newRank.overallRank;
                    player.ProjectedPoints = newRank.points;
                }
                else
                {
                    newRank = playerRanks.FirstOrDefault(p => AreEqualClean(p.player, player.Name));
                    if (newRank != null)
                    {
                        player.Rank = newRank.overallRank;
                        player.ProjectedPoints = newRank.points;
                    }
                    else
                    {
                        player.Rank = -1;
                        player.ProjectedPoints = -1;
                    }
                }

            }

            DraftFileHandler.WriteCsvFile(players, "FantasyPlayers.csv");
        }

        private static void UpdateAges(List<Player> players)
        {
            var playerAges = DraftFileHandler.ReadCsvFile<AgesTemp>("player_ages.csv");

            foreach (var player in players)
            {
                if (player.Age == default(int))
                {
                    var matchedPlayer = playerAges.FirstOrDefault(p => AreEqualClean(p.Name, player.Name));
                    if (matchedPlayer != null)
                    {
                        player.Age = matchedPlayer.Age;
                    }
                }
            }

            DraftFileHandler.WriteCsvFile(players, "FantasyPlayers.csv");
        }

        private static void UpdateHistories(List<Player> players)
        {
            var histories = new List<PlayerHistory>();

            var historyTemps = DraftFileHandler.ReadCsvFile<PlayerHistoryTemp>("years_2012_fantasy_fantasy.csv");
            historyTemps.AddRange(DraftFileHandler.ReadCsvFile<PlayerHistoryTemp>("years_2013_fantasy_fantasy.csv"));
            historyTemps.AddRange(DraftFileHandler.ReadCsvFile<PlayerHistoryTemp>("years_2014_fantasy_fantasy.csv"));

            foreach (var historyTemp in historyTemps)
            {
                var historyTemp1 = historyTemp;
                historyTemp1.Name = historyTemp1.Name.Replace("*", "").Replace("+", "");
                var matchedPlayers = players.Where(p => AreEqualClean(p.Name, historyTemp1.Name));
                PlayerHistory matchedPlayer;

                var enumerable = matchedPlayers as Player[] ?? matchedPlayers.ToArray();
                if (enumerable.Length > 1)
                {
                    var firstOrDefault = players.FirstOrDefault(p => AreEqualClean(p.Name + p.Team, historyTemp1.Name + historyTemp1.Team));
                    matchedPlayer = firstOrDefault != null
                        ? new PlayerHistory { PlayerId = firstOrDefault.PlayerId }
                        : new PlayerHistory();
                }
                else if (enumerable.Length == 1)
                {
                    matchedPlayer = new PlayerHistory { PlayerId = enumerable[0].PlayerId };
                }
                else
                {
                    continue;
                }

                histories.Add((PlayerHistory)matchedPlayer.InjectFrom(historyTemp1));
            }

            //var playersTemp = players.Where(p => !histories.Select(h => h.PlayerId).Contains(p.PlayerId));

            //foreach (var rookies in playersTemp)
            //{
            //    Console.WriteLine(rookies.Name);
            //}

            DraftFileHandler.WriteCsvFile(histories, "FantasyPlayersHistory.csv");
        }

        private static void UpdateByeWeeks(List<Player> players)
        {
            var schedules = DraftFileHandler.ReadCsvFile<TeamSchedule>("TeamSchedules.csv");
            var teamByeWeek = schedules.ToDictionary(schedule => schedule.Name, GetByeWeek);

            foreach (var player in players)
            {
                if (player.ByeWeek == default(int))
                {
                    player.ByeWeek = teamByeWeek[player.Team];
                }
            }

            DraftFileHandler.WriteCsvFile(players, "FantasyPlayers.csv");
        }

        private static int GetByeWeek(TeamSchedule schedule)
        {
            if (schedule.Week1 == "BYE") return 1;
            if (schedule.Week2 == "BYE") return 2;
            if (schedule.Week3 == "BYE") return 3;
            if (schedule.Week4 == "BYE") return 4;
            if (schedule.Week5 == "BYE") return 5;
            if (schedule.Week6 == "BYE") return 6;
            if (schedule.Week7 == "BYE") return 7;
            if (schedule.Week8 == "BYE") return 8;
            if (schedule.Week9 == "BYE") return 9;
            if (schedule.Week10 == "BYE") return 10;
            if (schedule.Week11 == "BYE") return 11;
            if (schedule.Week12 == "BYE") return 12;
            if (schedule.Week13 == "BYE") return 13;
            if (schedule.Week14 == "BYE") return 14;
            if (schedule.Week15 == "BYE") return 15;
            if (schedule.Week16 == "BYE") return 16;
            if (schedule.Week17 == "BYE") return 17;
            return 0;
        }

        private static bool AreEqualClean(string name1, string name2)
        {
            return String.Equals(name1.Replace(".", "").Replace(" ", "").Replace("'", ""), name2.Replace(".", "").Replace(" ", "").Replace("'", ""), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
