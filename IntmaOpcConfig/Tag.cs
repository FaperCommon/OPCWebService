using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Intma.OpcService.Config
{
    public class Tag 
    {
        public string ID { get; set; }
        public string TagName { get; set; }
        public string Group { get; set; }
        public Tag(XElement tag)
        {
            ID = tag.Element("ID").Value;
            TagName = tag.Element("TagName").Value;
        }
        public Tag()
        {
        }
    }
}
