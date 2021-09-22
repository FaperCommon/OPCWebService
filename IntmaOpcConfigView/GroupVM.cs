using Intma.OpcService.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Intma.OpcService.Config
{
    public class GroupVM : Notify
    {
        readonly Group _group;
        private ObservableCollection<TagVM> _tags;
        private bool _isSelected;

        public ObservableCollection<TagVM> Tags { get => _tags; set { _tags = value; OnPropertyChanged(); } }
        public TagVM SelectedTag { get; set; }
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }
        public string Name { get => _group.Name; set { _group.Name = value; OnPropertyChanged(); } }
        public ObservableCollection<ContextAction> Actions { get; set; }

        private bool CanOperate
        {
            get { return SelectedTag != null; }
        }

        public GroupVM(Group group)
        {
            _group = group;
            Tags = new ObservableCollection<TagVM>();
            foreach (var tag in _group.Tags)
            {
                Tags.Add(new TagVM(tag));
            }
        }

        public GroupVM()
        {
            _group = new Group();
            Tags = new ObservableCollection<TagVM>();
        }

        private void DeleteSelected()
        {
            _group.Tags.Remove(_group.Tags.First(a => (object)a.ID == SelectedTag.ID));
            Tags.Remove(SelectedTag);
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(param => DeleteSelected(), param => CanOperate);
                }
                return _deleteCommand;
            }
        }

        public void AddNew()
        {
            var tag = new Tag() { ID = $"Tag{Tags.Count}" };
            _group.Tags.Add(tag);
            Tags.Add(new TagVM(tag));
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
            _group.Tags.Clear();
            Tags.Clear();
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
