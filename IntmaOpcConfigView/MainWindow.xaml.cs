using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Intma.OpcService.Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConfigVM _config;
        public MainWindow()
        {
            InitializeComponent();
            _config = new ConfigVM();
            DataContext = _config; 
            EventLogInit();
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void EventLogInit()
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("IntmaOpcServiceWeb"))
                    System.Diagnostics.EventLog.CreateEventSource("IntmaOpcServiceWeb", "IntmaOpcServiceWeb_EventLog");
            }
            catch
            {
                MessageBox.Show("Не удалось зарегистрировать лог");
            }
        }

    }
}
