using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Intma.OpcService.Config
{
    /// <summary>
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window, INotifyPropertyChanged
    {
        private string _header;
        private string _input;

        public string Input { get => _input; set { _input = value; OnPropertyChanged(); } }
        public string Header { get => _header; set { _header = value; OnPropertyChanged(); } }
        public bool IsAdded { get; set; }
        public AddGroupWindow(string title, string header)
        {
            InitializeComponent();
            Title = title;
            Header = header;
            DataContext = this;
        }

        public AddGroupWindow(string title, string header, string input)
        {
            InitializeComponent();
            Title = title;
            Header = header;
            Input = input;
            DataContext = this;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Input))
            {
                MessageBox.Show("Поле ввода не может быть пустым!");
                return;
            }

            IsAdded = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
