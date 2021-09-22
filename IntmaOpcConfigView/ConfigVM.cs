using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace Intma.OpcService.Config
{
    public class ConfigVM : Notify
    {
        readonly Config _config;
        private GroupVM _selectedGroup;

        public string Server { get => _config.Server; set => _config.Server = value; }
        public int UpdateRate { get => _config.UpdateRate; set => _config.UpdateRate = value; }
        public BindingList<GroupVM> Groups { get; set; }
        public ObservableCollection<ContextAction> Actions { get; set; }
        public GroupVM SelectedGroup { get => _selectedGroup; set { _selectedGroup = value; OnPropertyChanged(); } }
        public ConfigVM()
        {
            try { 

                _config = new Config(System.Configuration.ConfigurationManager.AppSettings["configPath"]);
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            var wA = new AddGroupWindow("Редактирование группы","Имя группы:",SelectedGroup.Name);
            wA.ShowDialog();
            if (wA.IsAdded)
            {
                SelectedGroup.Name = wA.Input;
            }

        }

        private ICommand _updateSeviceCommand;
        public ICommand UpdateSeviceCommand
        {
            get
            {
                if (_updateSeviceCommand == null)
                {
                    _updateSeviceCommand = new RelayCommand(param => UpdateService(), param => true);
                }
                return _updateSeviceCommand;
            }
        }

        private void UpdateService()
        {
            try {
                WebRequest request = WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings.Get("adress"));
                WebResponse response = request.GetResponse();

                response.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                _config.Groups.Remove(_config.Groups.First(a => (object)a.Name == SelectedGroup.Name));
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

            var wA = new AddGroupWindow("Введите группу.шлюз.узел", "Группа.шлюз.узел:");
            wA.ShowDialog();
            if (!wA.IsAdded)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv) | *.csv";
            openFileDialog.DefaultExt = "csv";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var group = _config.ImportFromCSV(openFileDialog.FileName, wA.Input);
                    if (!Groups.Any(a => a.Name == group.Name))
                        Groups.Add(new GroupVM(group));
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
            var wA = new AddGroupWindow("Добавить группу","Имя группы:");
            wA.ShowDialog();
            if (wA.IsAdded)
            {
                var gr = new Group() { Name = wA.Input };
                _config.Groups.Add(gr);
                Groups.Add(new GroupVM(gr));
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
