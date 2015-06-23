namespace FileHandler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Serialization;
    using DraftEntities;

    public static class DraftFileHandler
    {
        public static List<T> ReadCsvFile<T>(string fileName, bool hasHeader = true)
            where T : class, new()
        {
            var objectList = new List<T>();
            var fileStream = new StreamReader(File.OpenRead(fileName));
            var firstLine = hasHeader;
            var columns = new Dictionary<string, int>();
            while (!fileStream.EndOfStream)
            {
                string line = fileStream.ReadLine();
                if (line != null)
                {
                    string[] values = line.Split(',');
                    if (firstLine)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            columns[values[i]] = i;
                        }
                        
                        firstLine = false;
                        continue;
                    }

                    objectList.Add(CreateObject<T>(columns, values));
                }
            }

            return objectList;
        }

        public static void WriteCsvFile<T>(List<T> objects, string fileName, bool hasHeader = true)
            where T : class, new()
        {
            //before your loop
            var csv = new StringBuilder();
            string line = string.Empty;

            if (hasHeader)
            {
                IList<PropertyInfo> props = new List<PropertyInfo>(typeof(T).GetProperties());
                line = props.Aggregate(line, (current, prop) => current + "," + string.Format("{0}", prop.Name));
                csv.AppendLine(string.Format("{0}{1}", line, Environment.NewLine));
            }

            //in your loop
            foreach (var o in objects)
            {
                IList<PropertyInfo> props = new List<PropertyInfo>(o.GetType().GetProperties());
                line = props.Aggregate(line, (current, prop) => current + "," + string.Format("{0}", prop.GetValue(o)));
                csv.Append(string.Format("{0}{1}", line, Environment.NewLine));
            }

            //after your loop
            File.WriteAllText(fileName, csv.ToString());
        }

        private static T CreateObject<T>(Dictionary<string, int> columns, string[] values)
            where T : class, new()
        {
            var newObject = new T();
            IList<PropertyInfo> props = new List<PropertyInfo>(typeof(T).GetProperties());

            foreach (PropertyInfo prop in props)
            {
                prop.SetValue(newObject, values[columns[prop.Name]]);
            }
            return newObject;
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