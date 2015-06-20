namespace FileHandler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using DraftEntities;

    public static class DraftFileHandler
    {
        public static List<Player> ReadPlayerFile(string fileName)
        {
            var playerList = new List<Player>();
            var fileStream = new StreamReader(File.OpenRead(fileName));

            while (!fileStream.EndOfStream)
            {
                string line = fileStream.ReadLine();
                if (line != null)
                {
                    string[] values = line.Split(',');

                    playerList.Add(new Player
                    {
                        AverageDraftPosition = Int32.Parse(values[0]),
                        Name = values[1],
                        Position = (PlayerPosition)Enum.Parse(typeof(PlayerPosition), values[2]),
                        Team = values[3],
                        ByeWeek = Int32.Parse(values[4]),
                        ProjectedPoints = Decimal.Parse(values[8])
                    });
                }
            }

            return playerList;
        }

        public static Theme ReadThemeFile(string fileName)
        {
            var reader = new XmlSerializer(typeof(Theme));
            var file = new StreamReader(fileName);
            var theme = (Theme)reader.Deserialize(file);
            file.Close();
            return theme;
        }

        public static void WriteThemeFile(Theme theme, string fileName)
        {
            var writer = new XmlSerializer(typeof(Theme));
            var file = new StreamWriter(fileName);
            writer.Serialize(file, theme);
            file.Close();
        }
    }
}