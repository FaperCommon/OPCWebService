using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Intma.OpcService.Config
{
    public class Config
    {

        readonly string _filepath;

        public string Server { get; set; }
        public int UpdateRate { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

        public Config(string filepath)
        {
            _filepath = filepath;
            if (!File.Exists(_filepath))
            {
                Groups = new ObservableCollection<Group>();
                Server = "OI.GATEWAY.3";
                UpdateRate = 3000;
                ConfingWrite();
            }
            ConfingRead(_filepath);
        }

        public void ConfingRead(string filepath)
        {
            Groups = new ObservableCollection<Group>();
            var doc = XDocument.Load(filepath);
            var el = doc.Element("Config");
            Server = el.Element("Server").Value;
            int per;
            if (Int32.TryParse(el.Element("UpdateRate").Value, out per))
                UpdateRate = per;
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
                      new XElement("UpdateRate",$"{ UpdateRate }"),
                      new XElement("Groups", Groups.Select((group, i) => new XElement($"Group{i}",
                            new XElement("Name", group.Name),
                            new XElement("Tags", group.Tags.Select((tag, j) => new XElement($"Tag{j}",
                                    new XElement("ID", tag.ID), new XElement("TagName", tag.TagName))))))));

            XDocument s = new XDocument(contacts);
            s.Save(_filepath);
        }

        /// <summary>
        /// Импортирует первые два столбца, filename - Имя группы
        /// </summary>
        public Group ImportFromCSV(string filepath, string node)
        {
            StreamReader sr = new StreamReader(filepath);
            string buff;
            Group gr;
            var name = filepath.Split('.')[0].Split('\\').Last();

            if (Groups.Any(a => a.Name == name))
            {
                gr = Groups.First(a => a.Name == name);
            }
            else
            {
                gr = new Group() { Name = name };
                Groups.Add(gr);
            }

            while ((buff = sr.ReadLine()) != null)
            {
                buff = buff.Replace("\"", "");
                var arr = buff.Split(';',',','\t');
                var id = $"{node}.{arr[0]}";
                if (!gr.Tags.Any(a => a.ID == id))
                    gr.Tags.Add(new Tag(){ID = id,  TagName = arr[1] });
            }

            return gr;
        }


    }
}
