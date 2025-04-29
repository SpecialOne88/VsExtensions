using PowerMacros.Entities;
using PowerMacros.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            get => _selectedMacro;
            set
            {
                _selectedMacro = value;
                OnPropertyChanged();
            }
        }

        private Visibility _listVisibility;
        public Visibility ListVisibility
        {
            get => _listVisibility;
            set
            {
                _listVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _editorVisibility;
        public Visibility EditorVisibility
        {
            get => _editorVisibility;
            set
            {
                _editorVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _editName;
        public string EditName
        {
            get => _editName;
            set
            {
                _editName = value;
                OnPropertyChanged();
            }
        }

        private string _editDescription;
        public string EditDescription
        {
            get => _editDescription;
            set
            {
                _editDescription = value;
                OnPropertyChanged();
            }
        }

        private Visibility _codeVisibility;
        public Visibility CodeVisibility
        {
            get => _codeVisibility;
            set
            {
                _codeVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _recorderVisibility;
        public Visibility RecorderVisibility
        {
            get => _recorderVisibility;
            set
            {
                _recorderVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _macroActionsText;
        public string MacroActionsText
        {
            get => _macroActionsText;
            set
            {
                _macroActionsText = value;
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
            get => _editMacroType;
            set
            {
                _editMacroType = value;
                OnPropertyChanged();
                MacroTypeVisibility();
            }
        }

        private string _editMacroCode;
        public string EditMacroCode
        {
            get => _editMacroCode;
            set
            {
                _editMacroCode = value;
                OnPropertyChanged();
            }
        }

        private string _editMacroOriginalName;
        public string EditMacroOriginalName
        {
            get => _editMacroOriginalName;
            set
            {
                _editMacroOriginalName = value;
                OnPropertyChanged();
            }
        }

        private Visibility _recordButtonVisibility = Visibility.Collapsed;
        public Visibility RecordButtonVisibility
        {
            get => _recordButtonVisibility;
            set
            {
                _recordButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _stopRecordButtonVisibility = Visibility.Collapsed;
        public Visibility StopRecordButtonVisibility
        {
            get => _stopRecordButtonVisibility;
            set
            {
                _stopRecordButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditEnabled = true;
        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set
            {
                _isEditEnabled = value;
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
        public RelayCommand StartRecordingCommand { get; }
        public RelayCommand StopRecordingCommand { get; }

        private List<MacroAction> _recordedActions = new List<MacroAction>();
        private bool _isRecording = false;
        private long _recordingStartTime = 0;

        MacroKeyboardHook _macroKeyboardHook = null;

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
            EditMacroCancelCommand = new RelayCommand(CancelEditMacro, CanEditMacro);
            StartRecordingCommand = new RelayCommand(StartRecording);
            StopRecordingCommand = new RelayCommand(StopRecording);

            LoadMacros();

            ListVisibility = Visibility.Visible;
            EditorVisibility = Visibility.Hidden;

            MacroTypeVisibility();
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

        private async void ApplyMacro(object parameter)
        {
            if (parameter is Macro macro)
            {
                try
                {
                    if (macro.MacroType == MacroType.Code)
                    {
                        Utils.TextEditor.InsertTextInCurrentView(macro.Code);
                    }
                    else if (macro.MacroType == MacroType.Action)
                    {
                        await InputSimulator.PlayRecordedMacro(macro.Actions);
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
                _recordedActions = macro.Actions.Select(x => x).ToList();

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
            MessageManager.Instance.Send(MacrosList.ToList());
        }

        private void AddNewMacro(object parameter)
        {
            EditDescription = string.Empty;
            EditMacroCode = string.Empty;
            EditMacroType = "Code";
            EditName = string.Empty;
            EditMacroOriginalName = null;

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

            if (EditMacroType.Equals("Action") && _recordedActions.Count == 0)
            {
                MessageBox.Show("No actions recorded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (EditMacroOriginalName != null)
            {
                var macro = MacrosList.FirstOrDefault(x => x.Name.Equals(EditMacroOriginalName, StringComparison.InvariantCultureIgnoreCase));
                var type = Enum.TryParse<MacroType>(EditMacroType, out MacroType value) ? value : MacroType.Code;
                if (macro != null)
                {
                    macro.Name = EditName;
                    macro.Description = EditDescription;
                    macro.MacroType = type;
                    macro.Code = type == MacroType.Code ? EditMacroCode : string.Empty;
                    macro.Actions = type == MacroType.Action ? _recordedActions : new List<MacroAction>();
                }
            }
            else
            {
                var type = Enum.TryParse<MacroType>(EditMacroType, out MacroType value) ? value : MacroType.Code;
                var newMacro = new Macro
                {
                    Name = EditName,
                    Description = EditDescription,
                    MacroType = type,
                    Code = type == MacroType.Code ? EditMacroCode : string.Empty,
                    Actions = type == MacroType.Action ? _recordedActions : new List<MacroAction>()
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
                !string.IsNullOrWhiteSpace(EditDescription) &&
                IsEditEnabled;
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

        private void MacroTypeVisibility()
        {
            if (EditMacroType == "Code")
            {
                CodeVisibility = Visibility.Visible;
                RecorderVisibility = Visibility.Hidden;
                RecordButtonVisibility = Visibility.Collapsed;
            }
            else if (EditMacroType == "Action")
            {
                CodeVisibility = Visibility.Hidden;
                RecorderVisibility = Visibility.Visible;
                RecordButtonVisibility = Visibility.Visible;
                if (SelectedMacro != null)
                {
                    MacroActionsText = Newtonsoft.Json.JsonConvert.SerializeObject(SelectedMacro.Actions, Newtonsoft.Json.Formatting.Indented);
                }
            }
        }

        private void StartRecording(object parameter)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            RecordButtonVisibility = Visibility.Collapsed;
            StopRecordButtonVisibility = Visibility.Visible;
            IsEditEnabled = false;
            _recordedActions = new List<MacroAction>();

            _isRecording = true;
            _recordingStartTime = Stopwatch.GetTimestamp();

            _macroKeyboardHook = new MacroKeyboardHook();
            _macroKeyboardHook.KeyDown += OnKeyDown;
            _macroKeyboardHook.KeyUp += OnKeyUp;
            _macroKeyboardHook.Install();
        }

        private void OnKeyDown(object sender, MacroKeyEventArgs e)
        {
            if (!_isRecording)
            {
                return;
            }

            _recordedActions.Add(new MacroAction
            {
                Type = MacroActionType.KeyDown,
                KeyCode = e.KeyCode,
                Modifiers = e.Modifiers,
                Delay = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - _recordingStartTime).TotalMilliseconds
            });
            _recordingStartTime = Stopwatch.GetTimestamp();
        }

        private void OnKeyUp(object sender, MacroKeyEventArgs e)
        {
            if (!_isRecording)
            {
                return;
            }

            _recordedActions.Add(new MacroAction
            {
                Type = MacroActionType.KeyUp,
                KeyCode = e.KeyCode,
                Modifiers = e.Modifiers,
                Delay = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - _recordingStartTime).TotalMilliseconds
            });
            _recordingStartTime = Stopwatch.GetTimestamp();
        }

        private void StopRecording(object parameter)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            RecordButtonVisibility = Visibility.Visible;
            StopRecordButtonVisibility = Visibility.Collapsed;
            IsEditEnabled = true;
            _isRecording = false;
            _macroKeyboardHook.KeyDown -= OnKeyDown;
            _macroKeyboardHook.KeyUp -= OnKeyUp;
            _macroKeyboardHook.Uninstall();
            _macroKeyboardHook = null;

            MacroActionsText = Newtonsoft.Json.JsonConvert.SerializeObject(_recordedActions, Newtonsoft.Json.Formatting.Indented);
        }

        private bool CanEditMacro(object parameter)
        {
            return IsEditEnabled;
        }
    }
}
