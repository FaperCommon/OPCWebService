using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Intma.OpcService.Config
{
    public class Config
    {

        readonly string _filepath = @"C:\IntmaOpcWebService.config";

        public string Server { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

        public Config()
        {
            if (!File.Exists(_filepath))
            {
                Groups = new ObservableCollection<Group>();
                Server = "OI.GATEWAY.3";
                ConfingWrite();
            }
            ConfingRead();
        }

        public void ConfingRead()
        {
            Groups = new ObservableCollection<Group>();
            var doc = XDocument.Load(_filepath);
            var el = doc.Element("Config");
            Server = el.Element("Server").Value;
            foreach (var group in el.Element("Groups").Elements())
            {
                Groups.Add(new Group(group));
            }
        }

        public void ConfingWrite()
        {
            XElement contacts =
                  new XElement("Config",
                      new XElement("Server", $"{Server}"),
                      new XElement("Groups", Groups.Select((group, i) => new XElement($"Group{i}",
                            new XElement("Name", group.Name),
                            new XElement("Tags", group.Tags.Select((tag, j) => new XElement($"Tag{j}",
                                    new XElement("ID", tag.ID), new XElement("SVGName", tag.SVGName ), new XElement("TagName", tag.TagName))))))));

            XDocument s = new XDocument(contacts);
            s.Save(_filepath);
        }

        /// <summary>
        /// Импортирует первые два столбца, filename - Имя группы
        /// </summary>
        public void ImportFromCSV(string filepath)
        {
            StreamReader sr = new StreamReader(filepath);
            string buff;
            var gr = new Group() { Name = filepath.Split('.')[0].Split('\\').Last() };
            while ((buff = sr.ReadLine()) != null)
            {
                var arr = buff.Split(';');
                gr.Tags.Add(new Tag(){ID = arr[0], SVGName = arr[1], TagName = arr[1] });
            }

            Groups.Add(gr);
        }

    }
}
