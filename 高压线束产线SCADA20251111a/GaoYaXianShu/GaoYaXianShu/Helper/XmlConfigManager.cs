using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DianJiaoJi.Helper
{
    public class XmlConfigManager<T> where T : class, new()
    {
        private readonly string _filePath;

        public XmlConfigManager(string filePath = "config.xml")
        {
            _filePath = filePath;
        }

        public T Load() => File.Exists(_filePath)
            ? Deserialize<T>(File.ReadAllText(_filePath))
            : new T();

        public void Save(T config) =>
            File.WriteAllText(_filePath, Serialize(config));

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }

        }

        private static string Serialize<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }

        }
    }
}
