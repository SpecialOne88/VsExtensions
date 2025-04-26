using PowerMacros.Entities;
using PowerMacros.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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

        private Visibility _listVisibility;
        public Visibility ListVisibility
        {
            get 
            { 
                return _listVisibility;
            }
            set 
            { 
                _listVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _editorVisibility;
        public Visibility EditorVisibility
        {
            get
            {
                return _editorVisibility;
            }
            set
            {
                _editorVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _editName;
        public string EditName
        {
            get 
            { 
                return _editName;
            }
            set 
            { 
                _editName = value;
                OnPropertyChanged();
            }
        }

        private string _editDescription;
        public string EditDescription
        {
            get
            {
                return _editDescription;
            }
            set
            {
                _editDescription = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> MacroTypes { get; } = new ObservableCollection<string>
        {
            "Code",
            "Action"
        };

        private string _editMacroType;
        public string EditMacroType
        {
            get
            {
                return _editMacroType;
            }
            set
            {
                _editMacroType = value;
                OnPropertyChanged();
            }
        }

        private string _editMacroCode;
        public string EditMacroCode
        {
            get
            {
                return _editMacroCode;
            }
            set
            {
                _editMacroCode = value;
                OnPropertyChanged();
            }
        }

        private string _editMacroOriginalName;
        public string EditMacroOriginalName
        {
            get
            {
                return _editMacroOriginalName;
            }
            set
            {
                _editMacroOriginalName = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ApplyCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand MoveUpCommand { get; }
        public RelayCommand MoveDownCommand { get; }
        public RelayCommand AddMacroCommand { get; }
        public RelayCommand ImportCommand { get; }
        public RelayCommand ExportCommand { get; }
        public RelayCommand EditMacroSaveCommand { get; }
        public RelayCommand EditMacroCancelCommand { get; }

        public MacrosWindowViewModel()
        {
            ApplyCommand = new RelayCommand(ApplyMacro);
            EditCommand = new RelayCommand(EditMacro);
            DeleteCommand = new RelayCommand(DeleteMacro);
            MoveUpCommand = new RelayCommand(MoveMacroUp, CanMoveMacroUp);
            MoveDownCommand = new RelayCommand(MoveMacroDown, CanMoveMacroDown);
            AddMacroCommand = new RelayCommand(AddNewMacro);
            ImportCommand = new RelayCommand(ImportMacros);
            ExportCommand = new RelayCommand(ExportMacros);
            EditMacroSaveCommand = new RelayCommand(SaveEditMacro, CanSaveEditMacro);
            EditMacroCancelCommand = new RelayCommand(CancelEditMacro);

            LoadMacros();

            ListVisibility = Visibility.Visible;
            EditorVisibility = Visibility.Hidden;
        }

        private void LoadMacros()
        {
            MacrosList.Clear();
            foreach (var item in MacroLoader.LoadMacrosFromSettings())
            {
                MacrosList.Add(item);
            }
            UpdateMacroShortcutAndSave();
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
                EditName = macro.Name;
                EditDescription = macro.Description;
                EditMacroType = macro.MacroType.ToString();
                EditMacroCode = macro.Code;
                EditMacroOriginalName = macro.Name;

                ListVisibility = Visibility.Hidden;
                EditorVisibility = Visibility.Visible;
            }
        }

        private void DeleteMacro(object parameter)
        {
            if (parameter is Macro macro)
            {
                if (MessageBox.Show($"Are you sure you want to delete {macro.Name}?", "Delete Macro", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MacrosList.Remove(macro);
                    UpdateMacroShortcutAndSave();
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
                UpdateMacroShortcutAndSave();
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
                UpdateMacroShortcutAndSave();
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

        private void UpdateMacroShortcutAndSave()
        {
            for (int i = 0; i < MacrosList.Count; i++)
            {
                MacrosList[i].Shortcut = $"Shift+Ctrl+M, {i + 1}";
                MacrosList[i].UpdatePreview();
            }
            OnPropertyChanged(nameof(MacrosList));
            MacroLoader.SaveMacrosToSettings(MacrosList.ToList());
        }

        private void AddNewMacro(object parameter)
        {
            EditDescription = string.Empty;
            EditMacroCode = string.Empty;
            EditMacroType = "Code";
            EditName = string.Empty;
            EditMacroOriginalName = null;

            // Open a new window to create a new macro
            ListVisibility = Visibility.Hidden;
            EditorVisibility = Visibility.Visible;
        }

        private void ImportMacros(object parameter)
        {
            try
            {
                var loadFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Import Macros",
                    Filter = "JSON Files (*.json)|*.json",
                    DefaultExt = "json",
                    Multiselect = false,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                // Show the dialog and check if the user selected a file
                if (loadFileDialog.ShowDialog() == true)
                {
                    if (MacrosList.Count > 0)
                    {
                        if (MessageBox.Show($"Are you sure you want to replace existing macros?", "Import Macros", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }

                    var filePath = loadFileDialog.FileName;
                    var macrosJson = System.IO.File.ReadAllText(filePath);

                    if (string.IsNullOrWhiteSpace(macrosJson))
                    {
                        MessageBox.Show("The selected file is empty or invalid.", "Import Macros", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var importedMacros = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Macro>>(macrosJson);
                    if (importedMacros != null)
                    {
                        MacrosList.Clear();
                        foreach (var macro in importedMacros)
                        {
                            MacrosList.Add(macro);
                        }
                        UpdateMacroShortcutAndSave();
                        MessageBox.Show("Macros imported successfully!", "Import Macros", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to import macros. Please check the file format.", "Import Macros", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred while importing macros. Please check the file format.", "Import Macros", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportMacros(object parameter)
        {
            try
            {
                if (MacrosList.Count == 0)
                {
                    MessageBox.Show("No macros to export.", "Export Macros", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Export Macros",
                    Filter = "JSON Files (*.json)|*.json",
                    DefaultExt = "json",
                    FileName = "Macros.json"
                };

                // Show the dialog and check if the user selected a file
                if (saveFileDialog.ShowDialog() == true)
                {
                    var macrosJson = Newtonsoft.Json.JsonConvert.SerializeObject(MacrosList, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(saveFileDialog.FileName, macrosJson);
                    MessageBox.Show("Macros exported successfully!", "Export Macros", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting macros: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveEditMacro(object parameter)
        {
            if (EditName.Contains(" "))
            {
                MessageBox.Show("Macro name cannot contain spaces.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (EditMacroOriginalName == null &&
                MacrosList.FirstOrDefault(x => x.Name.Equals(EditName, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                MessageBox.Show("Macro with this name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (EditMacroOriginalName != null &&
                !EditName.Equals(EditMacroOriginalName, StringComparison.InvariantCultureIgnoreCase) &&
                MacrosList.FirstOrDefault(x => x.Name.Equals(EditName, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                MessageBox.Show("Macro with this name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (EditMacroType.Equals("Code") && string.IsNullOrWhiteSpace(EditMacroCode))
            {
                MessageBox.Show("Code cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (EditMacroOriginalName != null)
            {
                var macro = MacrosList.FirstOrDefault(x => x.Name.Equals(EditMacroOriginalName, StringComparison.InvariantCultureIgnoreCase));
                if (macro != null)
                {
                    macro.Name = EditName;
                    macro.Description = EditDescription;
                    macro.MacroType = Enum.TryParse<MacroType>(EditMacroType, out MacroType value) ? value : MacroType.Code;
                    macro.Code = EditMacroCode;
                }
            }
            else
            {
                var newMacro = new Macro
                {
                    Name = EditName,
                    Description = EditDescription,
                    MacroType = Enum.TryParse<MacroType>(EditMacroType, out MacroType value) ? value : MacroType.Code,
                    Code = EditMacroCode
                };
                MacrosList.Add(newMacro);
            }

            UpdateMacroShortcutAndSave();
            ListVisibility = Visibility.Visible;
            EditorVisibility = Visibility.Hidden;
            EditDescription = string.Empty;
            EditMacroCode = string.Empty;
            EditMacroType = "Code";
            EditName = string.Empty;
            EditMacroOriginalName = null;
            MessageBox.Show("Macro saved successfully!", "Save Macro", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanSaveEditMacro(object parameter)
        {
            return !string.IsNullOrWhiteSpace(EditName) &&
                !string.IsNullOrWhiteSpace(EditMacroType) &&
                !string.IsNullOrWhiteSpace(EditDescription);
        }

        private void CancelEditMacro(object parameter)
        {
            EditDescription = string.Empty;
            EditMacroCode = string.Empty;
            EditMacroType = "Code";
            EditName = string.Empty;
            EditMacroOriginalName = null;

            ListVisibility = Visibility.Visible;
            EditorVisibility = Visibility.Hidden;
        }
    }
}
