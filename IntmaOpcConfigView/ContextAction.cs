using System.Windows.Input;
using System.Windows.Media;

namespace Intma.OpcService.Config
{
    public class ContextAction 
    {
        public string Name { get; set; }
        public ICommand Action { get; set; }
        public Brush Icon { get; set; }
    }
}
