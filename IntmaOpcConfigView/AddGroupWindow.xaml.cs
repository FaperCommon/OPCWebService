using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Intma.OpcService.Config;

namespace Intma.OpcService.Config
{
    /// <summary>
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        public Group Group { get; }
        public bool IsAdded { get; set; }
        public AddGroupWindow()
        {
            InitializeComponent();
            Group = new Group();
            DataContext = Group;
        }

        public AddGroupWindow(GroupVM group)
        {
            InitializeComponent();
            Group = new Group() { Name = group.Name };
            DataContext = Group;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Group.Name))
            {
                MessageBox.Show("Поле с именем должно быть заполнено!");
                return;
            }

            MessageBox.Show("Запись успешно сохранена!");
            IsAdded = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
