using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace OpcServiceWeb
{
    public class Config
    {
        public string Server { get; set; }
        public ObservableCollection<string> Tags { get; set; }

        string filepath = @"C:\IntmaOpcWebService.config";

        public Config()
        {
            Tags = new ObservableCollection<string>();
            if (!File.Exists(filepath))
            {
                Server = "OI.MBTCP.1";
                ConfingWrite();
            }
            ConfingRead();
        }

        public void ConfingRead()
        {
            Tags = new ObservableCollection<string>();
            var doc = XDocument.Load(filepath);
            var el = doc.Element("Config");
            Server = el.Element("Server").Value;
            foreach (var tag in el.Element("Tags").Elements())
            {
                Tags.Add(tag.Value);
            }
        }

        public void ConfingWrite()
        {
            XElement contacts =
                  new XElement("Config",
                      new XElement("Server", $"{Server}"),
                      new XElement("Tags", Tags.Select((tag, i) => new XElement($"Tag{i}", tag))));

            XDocument s = new XDocument(contacts);
            s.Save(filepath);
        }
        /// <summary>
        /// Импортирует первый столбец
        /// </summary>
        public void ImportFromCSV(string filepath)
        {
            StreamReader sr = new StreamReader(filepath);
            string buff;
            while ((buff = sr.ReadLine()) != null)
            {
                //System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator
                Tags.Add(buff.Split(';')[0]);
            }
        }
    }
}