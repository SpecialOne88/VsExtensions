using PowerMacros.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PowerMacros.ViewModels
{
    public class MacrosWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Macro> MacrosList { get; set; } = new ObservableCollection<Macro>();

        private Macro _selectedMacro;
        public Macro SelectedMacro
        {
            get { return _selectedMacro; }
            set
            {
                _selectedMacro = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand MoveUpCommand { get; }
        public RelayCommand MoveDownCommand { get; }


        public MacrosWindowViewModel()
        {
            EditCommand = new RelayCommand(EditMacro);
            DeleteCommand = new RelayCommand(DeleteMacro);
            MoveUpCommand = new RelayCommand(MoveMacroUp, CanMoveMacroUp);
            MoveDownCommand = new RelayCommand(MoveMacroDown, CanMoveMacroDown);

            LoadMacros();
        }

        private void LoadMacros()
        {
            MacrosList.Clear();
            MacrosList.Add(new Macro { Name = "Macro1", Description = "Description1", MacroType = MacroType.Code, Code = "Code1" });
            MacrosList.Add(new Macro { Name = "Macro2", Description = "Description2", MacroType = MacroType.Code, Code = "Code2" });
            MacrosList.Add(new Macro { Name = "Macro3", Description = "Description3", MacroType = MacroType.Code, Code = "Code3" });
            UpdateMacroShortcut();
            OnPropertyChanged(nameof(MacrosList));
        }

        private void EditMacro(object parameter)
        {
            if (parameter is Macro macro)
            {
                MessageBox.Show($"Editing macro: {macro.ToString()}", "Edit Macro");
            }
        }

        private void DeleteMacro(object parameter)
        {
            if (parameter is Macro macro)
            {
                if (MessageBox.Show($"Are you sure you want to delete {macro.Name}?", "Delete Macro", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MacrosList.Remove(macro);
                    UpdateMacroShortcut();
                }
            }
        }

        private void MoveMacroUp(object parameter)
        {
            if (parameter is Macro macro)
            {
                int index = MacrosList.IndexOf(macro);
                if (index > 0)
                {
                    MacrosList.Move(index, index - 1);
                }
                UpdateMacroShortcut();
            }
        }

        private bool CanMoveMacroUp(object parameter)
        {
            if (parameter is Macro macro)
            {
                return MacrosList.IndexOf(macro) > 0;
            }
            return false;
        }

        private void MoveMacroDown(object parameter)
        {
            if (parameter is Macro macro)
            {
                int index = MacrosList.IndexOf(macro);
                if (index < MacrosList.Count - 1)
                {
                    MacrosList.Move(index, index + 1);
                }
                UpdateMacroShortcut();
            }
        }

        private bool CanMoveMacroDown(object parameter)
        {
            if (parameter is Macro macro)
            {
                return MacrosList.IndexOf(macro) < MacrosList.Count - 1;
            }
            return false;
        }

        private void UpdateMacroShortcut()
        {
            for (int i = 0; i < MacrosList.Count; i++)
            {
                MacrosList[i].Shortcut = $"Shift+Ctrl+{i + 1}";
            }
            MacrosList = new ObservableCollection<Macro>(MacrosList.Select(a => a));
            OnPropertyChanged(nameof(MacrosList));
        }
    }
}
