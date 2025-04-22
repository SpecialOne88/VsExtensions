using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using PowerMacros.Entities;
using PowerMacros.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public RelayCommand ApplyCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand MoveUpCommand { get; }
        public RelayCommand MoveDownCommand { get; }
        public RelayCommand AddMacroCommand { get; }


        public MacrosWindowViewModel()
        {
            ApplyCommand = new RelayCommand(ApplyMacro);
            EditCommand = new RelayCommand(EditMacro);
            DeleteCommand = new RelayCommand(DeleteMacro);
            MoveUpCommand = new RelayCommand(MoveMacroUp, CanMoveMacroUp);
            MoveDownCommand = new RelayCommand(MoveMacroDown, CanMoveMacroDown);
            AddMacroCommand = new RelayCommand(AddNewMacro);

            LoadMacros();
        }

        private void LoadMacros()
        {
            MacrosList.Clear();
            foreach (var item in MacroLoader.LoadMacrosFromSettings())
            {
                MacrosList.Add(item);
            }
            UpdateMacroShortcut();
        }

        private void ApplyMacro(object parameter)
        {
            if (parameter is Macro macro)
            {
                try
                {
                    if (macro.MacroType == MacroType.Code)
                    {
                        TextEditor.InsertTextInCurrentView(macro.Code);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to apply macro: {ex.Message}", "Error");
                }
            }
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
                MacrosList[i].Shortcut = $"Shift+Ctrl+M, {i + 1}";
            }
            OnPropertyChanged(nameof(MacrosList));
            MacroLoader.SaveMacrosToSettings(MacrosList.ToList());
        }

        private void AddNewMacro(object parameter)
        {
            var newMacro = new Macro
            {
                Name = "New Macro",
                Description = "Description",
                Code = "if (1 == 1)\n{\n\treturn 0;\n}\n",
                MacroType = MacroType.Code,
                Shortcut = $"Shift+Ctrl+M, {MacrosList.Count + 1}"
            };
            MacrosList.Add(newMacro);
            UpdateMacroShortcut();
        }
    }
}
