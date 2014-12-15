namespace FileReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
                    AverageDraftPosition = Int32.Parse(values[0]),
                    Name = values[1].ToString(),
                    Position = (PlayerPosition)Enum.Parse(typeof(PlayerPosition), values[2]),
                    Team = values[3].ToString(),
                    ByeWeek = Int32.Parse(values[4]),
                    ProjectedPoints = Decimal.Parse(values[8])
                });
            }

            return playerList;
        }
    }
}
