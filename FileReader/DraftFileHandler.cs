﻿namespace FileReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DraftEntities;

    public static class DraftFileHandler
    {
        public static List<Player> ReadFile(string fileName)
        {
            var playerList = new List<Player>();
            var fileStream = new StreamReader(File.OpenRead(fileName));

            while (!fileStream.EndOfStream)
            {
                var line = fileStream.ReadLine();
                var values = line.Split(',');

                playerList.Add(new Player
                {
                    ADP = Int32.Parse(values[0]),
                    Name = values[1].ToString(),
                    Position = (PlayerPosition)Enum.Parse(typeof(PlayerPosition), values[2]),
                    Team = values[3].ToString(),
                    ByeWeek = Int32.Parse(values[4]),
                    YahooADP = Int32.Parse(values[5]),
                    ESPNADP = Int32.Parse(values[6]),
                    CBSADP = Int32.Parse(values[7]),
                    ProjectedPoints = Decimal.Parse(values[8])
                });
            }

            return playerList;
        }

        public static void SaveFile(string fileName, List<Player> players)
        {
            var csv = new StringBuilder();

            foreach (Player player in players)
            {
                csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", player.ADP, player.Name, player.Position.ToString(), player.Team, player.ByeWeek, 
                    player.YahooADP, player.ESPNADP, player.CBSADP, player.ProjectedPoints));
            }

            File.WriteAllText(fileName, csv.ToString());
        }
    }
}
