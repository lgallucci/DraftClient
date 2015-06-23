namespace DataFileCreator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DraftClient.ViewModel;
    using FileHandler;
    using Omu.ValueInjecter;

    public class PlayerHistoryTemp
    {
        public string Name { get; set; }
        public int Year{ get; set; }
        public int Team{ get; set; }
        public decimal Week1Points { get; set; }
        public decimal Week2Points { get; set; }
        public decimal Week3Points { get; set; }
        public decimal Week4Points { get; set; }
        public decimal Week5Points { get; set; }
        public decimal Week6Points { get; set; }
        public decimal Week7Points { get; set; }
        public decimal Week8Points { get; set; }
        public decimal Week9Points { get; set; }
        public decimal Week10Points { get; set; }
        public decimal Week11Points { get; set; }
        public decimal Week12Points { get; set; }
        public decimal Week13Points { get; set; }
        public decimal Week14Points { get; set; }
        public decimal Week15Points { get; set; }
        public decimal Week16Points { get; set; }
        public decimal Week17Points { get; set; }
    }

    public class DataFileCreator
    {
        public static void Main()
        {
            var players = DraftFileHandler.ReadCsvFile<Player>("FantasyPlayers.csv");

            UpdateHistories(players);
            UpdateByeWeeks(players);
        }

        private static void UpdateHistories(List<Player> players)
        {
            var histories = new List<PlayerHistory>();

            var historyTemps = DraftFileHandler.ReadCsvFile<PlayerHistoryTemp>("FantasyPlayersHistory.csv");

            foreach (var historyTemp in historyTemps)
            {
                var temp1 = historyTemp;
                var matchedPlayers = players.Where(p => AreEqualClean(p.Name, temp1.Name));
                PlayerHistory matchedPlayer;

                var enumerable = matchedPlayers as Player[] ?? matchedPlayers.ToArray();
                if (enumerable.Length > 1)
                {
                    var firstOrDefault = players.FirstOrDefault(p => AreEqualClean(p.Name + p.Team, temp1.Name + temp1.Team));
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
                    matchedPlayer = new PlayerHistory();
                }

                histories.Add((PlayerHistory)matchedPlayer.InjectFrom(historyTemps));
            }

            DraftFileHandler.WriteCsvFile(histories, "FantasyPlayersHistory.csv");
        }

        private static void UpdateByeWeeks(List<Player> players)
        {
            
        }

        private static bool AreEqualClean(string name1, string name2)
        {
            return String.Equals(name1.Replace(".", "").Replace(" ", ""), name2.Replace(".", "").Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
