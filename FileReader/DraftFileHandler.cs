namespace FileHandler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
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
            fileStream.Close();
            return objectList;
        }

        public static void WriteCsvFile<T>(List<T> objects, string fileName, bool hasHeader = true)
            where T : class, new()
        {
            //before your loop
            var csv = new StringBuilder();
            string line = string.Empty;
            IList<PropertyInfo> props = new List<PropertyInfo>(typeof (T).GetProperties());

            if (hasHeader)
            {
                csv.AppendLine(string.Join(",", props.Select(p => p.Name)));
            }

            //in your loop
            foreach (var o in objects)
            {
                var o1 = o;
                csv.AppendLine(string.Join(",", props.Select(p => p.GetValue(o1))));
            }

            //after your loop
            File.WriteAllText(fileName, csv.ToString());
        }

        private static T CreateObject<T>(Dictionary<string, int> columns, string[] values)
            where T : class, new()
        {
            var newObject = new T();
            IList<PropertyInfo> props = new List<PropertyInfo>(typeof (T).GetProperties());

            foreach (PropertyInfo prop in props)
            {
                prop.SetValue(newObject,
                    typeof (DraftFileHandler).GetMethod("ConvertWithEnum")
                        .MakeGenericMethod(prop.PropertyType)
                        .Invoke(null, new object[] {values[columns[prop.Name]]}));
            }
            return newObject;
        }

        public static T ConvertWithEnum<T>(String value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return default(T);

                if (typeof (T).IsEnum)
                    return (T) Enum.Parse(typeof (T), value);

                return (T) Convert.ChangeType(value, typeof (T));
            }
            catch (FormatException)
            {
                return default(T);
            }
        }

        public static T ReadFile<T>(string fileName)
        {
            fileName += ".dc";

            var reader = new BinaryFormatter();
            var file = new StreamReader(fileName);
            var readObject = (T) reader.Deserialize(file.BaseStream);
            file.Close();
            return readObject;
        }

        public static void WriteFile<T>(T data, string fileName)
        {
            fileName += ".dc";

            var writer = new BinaryFormatter();
            var file = new StreamWriter(fileName);
            writer.Serialize(file.BaseStream, data);
            file.Close();
        }

        public static string[] GetFilesWithPrefix(string prefix)
        {
            var localDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Directory.GetFiles(localDirectory ?? ".", string.Format("{0}_*", prefix));
        }
    }
}