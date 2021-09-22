using Intma.OpcService.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Intma.OpcService.Config
{
    public class TagVM 
    {
        readonly Tag _tag;

        public string ID { get => _tag.ID; set { _tag.ID = value; } }
        public string TagName { get => _tag.TagName; set => _tag.TagName = value; }

        public TagVM(Tag tag)
        {
            _tag = tag;
        }

        public TagVM()
        {
            _tag = new Tag();
        }
    }
}
