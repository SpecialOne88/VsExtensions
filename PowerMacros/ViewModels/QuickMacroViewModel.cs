using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PowerMacros.Entities;
using PowerMacros.Utils;
using PowerMacros.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PowerMacros.ViewModels
{
    public class QuickMacroViewModel : ViewModelBase
    {
        public ObservableCollection<Macro> MacrosList { get; set; } = new ObservableCollection<Macro>();
        public ObservableCollection<string> FilteredMacros { get; set; } = new ObservableCollection<string>();

        private string _macroSearchText;
        public string MacroSearchText
        {
            get => _macroSearchText;
            set
            {
                _macroSearchText = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand EnterKeyCommand { get; }

        private AsyncPackage _package;

        private bool _shouldClose = true;
        public bool ShouldClose
        {
            get => _shouldClose;
            set
            {
                _shouldClose = value;
                OnPropertyChanged();
            }
        }

        public QuickMacroViewModel()
        {
            EnterKeyCommand = new RelayCommand(ExecuteMacroByName);
            LoadMacros();
            MessageManager.Instance.Subscribe<List<Macro>>(this, macros =>
            {
                LoadMacros();
            });
        }

        public void SetPackage(AsyncPackage package)
        {
            _package = package;
        }

        private void LoadMacros()
        {
            MacrosList.Clear();
            foreach (var item in MacroLoader.LoadMacrosFromSettings())
            {
                MacrosList.Add(item);
            }
            OnPropertyChanged(nameof(MacrosList));
            UpdateFilteredMacros();
        }

        public void ExecuteMacroByName(object parameter)
        {
            string macroName = MacroSearchText;
            var macro = MacrosList.FirstOrDefault(m => m.Name.Equals(macroName, System.StringComparison.InvariantCultureIgnoreCase));
            if (macro != null)
            {
                // Execute the macro
                if (macro.MacroType == MacroType.Code)
                {
                    TextEditor.InsertTextInCurrentView(macro.Code);
                    if (ShouldClose)
                    {
                        CloseToolWindow();
                    }
                }
                else
                {
                    MessageBox.Show($"Macro '{macroName}' executed.", "Quick Macro", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show($"Macro '{macroName}' not found.", "Quick Macro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateFilteredMacros()
        {
            FilteredMacros.Clear();
            FilteredMacros.Add("");
            foreach (var macro in MacrosList)
            {
                FilteredMacros.Add(macro.Name);
            }
            OnPropertyChanged(nameof(FilteredMacros));
        }

        private void CloseToolWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var window = _package.FindToolWindow(typeof(QuickMacro), 0, false);
            if (window?.Frame != null && window.Frame is IVsWindowFrame windowFrame)
            {
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave));
            }
        }
    }
}
