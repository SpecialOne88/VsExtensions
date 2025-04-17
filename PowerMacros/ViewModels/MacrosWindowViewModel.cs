using PowerMacros.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public MacrosWindowViewModel()
        {
            LoadMacros();
        }

        private void LoadMacros()
        {
            MacrosList.Clear();
            MacrosList.Add(new Macro { Name = "Macro1", Description = "Description1", MacroType = MacroType.Code, Code = "Code1" });
            MacrosList.Add(new Macro { Name = "Macro2", Description = "Description2", MacroType = MacroType.Code, Code = "Code2" });
            OnPropertyChanged(nameof(MacrosList));
        }
    }
}
