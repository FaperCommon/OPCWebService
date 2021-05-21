using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace Intma.OpcService.Config
{
    public class Group
    {
        public string Name { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }

        public Group(XElement group)
        {
            Tags = new ObservableCollection<Tag>();
            Name = group.Element("Name").Value;
            foreach (var tag in group.Element("Tags").Elements())
            {
                Tags.Add(new Tag(tag) { Group = Name });
            }
        }

        public Group()
        {
            Tags = new ObservableCollection<Tag>();
        }
    }
}
