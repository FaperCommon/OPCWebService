using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Intma.OpcService.Config
{
    public class ConfigVM : Notify
    {
        readonly Config _config;
        private GroupVM _selectedGroup;

        public string Server { get => _config.Server; set => _config.Server = value; }
        public BindingList<GroupVM> Groups { get; set; }
        public ObservableCollection<ContextAction> Actions { get; set; }
        public GroupVM SelectedGroup { get => _selectedGroup; set { _selectedGroup = value; OnPropertyChanged(); } }
        public ConfigVM()
        {
            _config = new Config();
            Groups = new BindingList<GroupVM>();
            Groups.ListChanged += Groups_ListChanged;
            Actions = new ObservableCollection<ContextAction>() {
                new ContextAction() { Name = "Добавить тэг", Action = AddNewTagCommand },
                new ContextAction() { Name = "Редактировать", Action = ChangePropertiesCommand },
                new ContextAction() { Name = "Удалить", Action = RemoveGroupCommand }
            };

            foreach (var group in _config.Groups)
            {
                Groups.Add(new GroupVM(group));
            }

            if(Groups.Count != 0)
            {
                Groups[0].IsSelected = true;
            }
        }

        private void Groups_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.NewIndex < Groups.Count && Groups.Count > 0) { 
                if (Groups[e.NewIndex].IsSelected)
                {
                    SelectedGroup = Groups[e.NewIndex];
                }
            }
            else if (Groups.Count == 0) { 
                    SelectedGroup = null;
            }
        }

        private void SaveConfig()
        {
            _config.ConfingWrite();
            MessageBox.Show("Успешно сохранено!");
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param => SaveConfig(), param => true);
                }
                return _saveCommand;
            }
        }
        private void AddNewTag()
        {
            SelectedGroup.AddNew();
        }

        private ICommand _addNewTagCommand;
        public ICommand AddNewTagCommand
        {
            get
            {
                if (_addNewTagCommand == null)
                {
                    _addNewTagCommand = new RelayCommand(param => AddNewTag(), param => true);
                }
                return _addNewTagCommand;
            }
        }
        private void ChangeProperties()
        {
            var wA = new AddGroupWindow(SelectedGroup);
            wA.ShowDialog();
            if (wA.IsAdded)
            {
                SelectedGroup.Name = wA.Group.Name;
            }

        }

        private ICommand _changePropertiesCommand;
        public ICommand ChangePropertiesCommand
        {
            get
            {
                if (_changePropertiesCommand == null)
                {
                    _changePropertiesCommand = new RelayCommand(param => ChangeProperties(), param => true);
                }
                return _changePropertiesCommand;
            }
        }

        private void RemoveGroup()
        {
            try { 
                Groups.Remove(SelectedGroup);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _removeGroupCommand;
        public ICommand RemoveGroupCommand
        {
            get
            {
                if (_removeGroupCommand == null)
                {
                    _removeGroupCommand = new RelayCommand(param => RemoveGroup(), param => true);
                }
                return _removeGroupCommand;
            }
        }

        private void ImportCSV()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv) | *.csv";
            openFileDialog.DefaultExt = "csv";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _config.ImportFromCSV(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private ICommand _importCSVCommand;
        public ICommand ImportCSVCommand
        {
            get
            {
                if (_importCSVCommand == null)
                {
                    _importCSVCommand = new RelayCommand(param => ImportCSV(), param => true);
                }
                return _importCSVCommand;
            }
        }

        private void AddNew()
        {
            var wA = new AddGroupWindow();
            wA.ShowDialog();
            if (wA.IsAdded)
            {
                _config.Groups.Add(wA.Group);
                Groups.Add(new GroupVM(wA.Group));
            }
        }

        private ICommand _addNewCommand;

        public ICommand AddNewCommand
        {
            get
            {
                if (_addNewCommand == null)
                {
                    _addNewCommand = new RelayCommand(param => AddNew(), param => true);
                }
                return _addNewCommand;
            }
        }

        private void RemoveAll()
        {
            try { 
                Groups.Clear();
                _config.Groups.Clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _removeAllCommand;

        public ICommand RemoveAllCommand
        {
            get
            {
                if (_removeAllCommand == null)
                {
                    _removeAllCommand = new RelayCommand(param => RemoveAll(), param => true);
                }
                return _removeAllCommand;
            }
        }
    }
}
